using frznUpload.Shared;
using frznUpload.Web.Data;
using frznUpload.Web.Server.Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Web.Server
{
	class ServerService : IHostedService
	{
		static X509Certificate2 Cert;
		static List<(IServiceScope, Client)> clients = new();
		CancellationTokenSource tokenSource = new CancellationTokenSource();
		TaskCompletionSource completionSource = new TaskCompletionSource();
		static bool Verbose = false;
		private readonly ILogger<ServerService> logger;
		private readonly IServiceProvider provider;
		private readonly IConfiguration config;

		public ServerService(ILogger<ServerService> logger, IServiceProvider provider, IConfiguration config)
		{
			this.logger = logger;
			this.provider = provider;
			this.config = config;
		}

		private async Task<bool> ConnectionAcceptor(TcpListener tcp)
		{
			FileHandler.Init(config);
			while (true)
			{
				Task<TcpClient> accept = tcp.AcceptTcpClientAsync();

				await Task.WhenAny(accept, completionSource.Task);
				tokenSource.Token.ThrowIfCancellationRequested();
				TcpClient cli = await accept;

				logger.LogInformation("Connection established with " + cli.Client.RemoteEndPoint);

				try
				{
					IServiceScope scope = provider.CreateScope();

					DatabaseHandler db = scope.ServiceProvider.GetRequiredService<DatabaseHandler>();
					CertificateHandler certHandler = scope.ServiceProvider.GetRequiredService<CertificateHandler>();
					ILogger<Client> log = scope.ServiceProvider.GetRequiredService<ILogger<Client>>();

					bool verbose = config.GetValue("Verbose", false);

					var Client = new Client(cli, db, log, certHandler, verbose);

					clients.Add((scope, Client));

					Client.OnDispose += Client_OnDispose;

					Client.Start();
				}
				catch (Exception)
				{
				}
			}

		}

		private void Client_OnDispose(object sender, EventArgs e)
		{
			clients.RemoveAll(t => t.Item2 == sender as Client);
			logger.LogDebug("Removed client");
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			tokenSource.Token.Register(() => completionSource.TrySetCanceled());

			logger.LogInformation("Server version: " + MessageHandler.Version);

			IPAddress address = IPAddress.Any;
			var listener = new TcpListener(address, 22340);


			string certdir = "../certs/cert.pfx";

#if DEBUG
			certdir = "fritzen.xyz.pfx";
#endif
			logger.LogInformation("Loading keyfile: " + certdir);

			Cert = new X509Certificate2(certdir, "FECBA15DE2919B0FF055E2C0A513261399B894691F208FE8AD54878824390902B2FC2753354FF173747F8B8079353ABCA10DEAED03482E419087CD044A5868F6", X509KeyStorageFlags.Exportable);


			AsymmetricCipherKeyPair keypair = DotNetUtilities.GetRsaKeyPair((Cert.PrivateKey as RSA));
			var randomGenerator = new CryptoApiRandomGenerator();
			var random = new SecureRandom(randomGenerator);
			var certificateGenerator = new X509V3CertificateGenerator();
			BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
			certificateGenerator.SetSerialNumber(serialNumber);


			var subjectDN = new X509Name("CN=frznUpload.Client");
			var issuerDN = new X509Name("CN=fritzen.xyz");
			certificateGenerator.SetIssuerDN(issuerDN);
			certificateGenerator.SetSubjectDN(subjectDN);

			DateTime notBefore = DateTime.UtcNow.Date;
			DateTime notAfter = notBefore.AddDays(2);

			certificateGenerator.SetNotBefore(notBefore);
			certificateGenerator.SetNotAfter(notAfter);

			AsymmetricCipherKeyPair subjectKeyPair;
			var keyGenerationParameters = new KeyGenerationParameters(random, 2048);
			var keyPairGenerator = new RsaKeyPairGenerator();
			keyPairGenerator.Init(keyGenerationParameters);
			subjectKeyPair = keyPairGenerator.GenerateKeyPair();

			certificateGenerator.SetPublicKey(subjectKeyPair.Public);

			AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;
			ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", keypair.Private, random);
			// Selfsign certificate
			Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(signatureFactory);

			var store = new Pkcs12Store();
			string friendlyName = certificate.SubjectDN.ToString();
			var certificateEntry = new X509CertificateEntry(certificate);
			store.SetCertificateEntry(friendlyName, certificateEntry);
			store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });
			const string password = "password";

			var stream = new MemoryStream();
			store.Save(stream, password.ToCharArray(), random);

			File.WriteAllBytes("client.pfx", stream.ToArray());

			var x509 = new X509Certificate2(stream.ToArray(), password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

			string serial = string.Concat(serialNumber.ToByteArray().Select(b => b.ToString("X2")));

			listener.Start();

			logger.LogInformation("Server initialized");

			_ = Task.Run(() => ConnectionAcceptor(listener));
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			tokenSource.Cancel();
		}
	}
}
