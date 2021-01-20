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
				_ = Task.Run(() =>
				{
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
					catch (Exception e)
					{
						logger.LogError(e, "Error while initializing client");
					}
				});
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
