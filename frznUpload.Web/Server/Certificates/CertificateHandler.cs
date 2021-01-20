using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace frznUpload.Web.Server.Certificates
{
	public class CertificateHandler
	{
		const string pemCertName = "cert";
		const string pemPrivkeyName = "privkey";

		private readonly IConfiguration config;
		List<CertificateAndKey> archive;
		CertificateAndKey currentCert;

		public System.Security.Cryptography.X509Certificates.X509Certificate Certificate => currentCert.X509Certificate;

		public CertificateHandler(IConfiguration configuration)
		{
			config = configuration;
			LoadCertificates();
		}

		private void LoadCertificates()
		{
			IConfigurationSection section = config.GetSection("Certificates");
			if (section.GetValue("UsePem", false))
				LoadCertificatesPem(section);
			else
				LoadCertificatesPfx(section);

		}


		public VerificationResult Verify(System.Security.Cryptography.X509Certificates.X509Certificate subject)
		{
			return Verify(DotNetUtilities.FromX509Certificate(subject));
		}

		public VerificationResult Verify(Org.BouncyCastle.X509.X509Certificate subject)
		{
			if (currentCert.Verify(subject))
				return VerificationResult.Valid;

			foreach (CertificateAndKey cert in archive)
			{
				if (cert.Verify(subject))
					return VerificationResult.ValidArchival;
			}

			return VerificationResult.Invalid;
		}

		public CertificationResult Certify(byte[][] publicKey)
		{


			var randomGenerator = new CryptoApiRandomGenerator();
			var random = new SecureRandom(randomGenerator);
			var certificateGenerator = new X509V3CertificateGenerator();
			BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
			certificateGenerator.SetSerialNumber(serialNumber);
			DateTime notBefore = DateTime.UtcNow.Date;
			DateTime notAfter = notBefore.AddDays(10);
			var subjectDN = new X509Name("CN=fritzenUpload.Client");
			var issuerDN = new X509Name("CN=fritzenUpload");
			certificateGenerator.SetIssuerDN(issuerDN);
			certificateGenerator.SetSubjectDN(subjectDN);


			certificateGenerator.SetNotBefore(notBefore);
			certificateGenerator.SetNotAfter(notAfter);

			var subjectPublic = new RsaKeyParameters(false, new BigInteger(publicKey[1]), new BigInteger(publicKey[0]));
			certificateGenerator.SetPublicKey(subjectPublic);


			// Selfsign certificate
			Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(currentCert.GetFactory(random));

			string serial = string.Concat(serialNumber.ToByteArray().Select(b => b.ToString("X2")));

			TextWriter textWriter = new StringWriter();
			PemWriter writer = new(textWriter);
			writer.WriteObject(certificate);
			textWriter.Flush();
			byte[] data = Encoding.UTF8.GetBytes(textWriter.ToString());

			var store = new Pkcs12Store();
			var certificateEntry = new X509CertificateEntry(certificate);
			string friendlyName = certificate.SubjectDN.ToString();
			store.SetCertificateEntry(friendlyName, certificateEntry);
			//TODO change encoding from pfx to something more efficient
			var stream = new MemoryStream();
			store.Save(stream, "".ToCharArray(), random);


			return new CertificationResult() { Rawdata = stream.ToArray(), SerialNumber = serial, ValidUntil = notAfter };
		}

		private void LoadCertificatesPem(IConfigurationSection section)
		{
			archive = new();
			string currentPath = section.GetValue<string>("CurrentPath") ?? throw new ArgumentNullException("Config: Certificates/CurrentPath");

			currentCert = FromPem(currentPath) ?? throw new FileNotFoundException("Current Certificate not found");

			if (section.GetValue<string>("ArchivePath") is string archivePath)
			{
				for (int i = 1; ; i++)
				{
					CertificateAndKey cert = FromPem(archivePath, i);
					if (cert is null)
						break;
					archive.Add(cert);
				}
			}
		}

		private void LoadCertificatesPfx(IConfigurationSection section)
		{
			archive = new();
			string password = section.GetValue<string>("Password") ?? throw new ArgumentNullException("Config: Certificates/Password");
			string currentPath = section.GetValue<string>("CurrentPath") ?? throw new ArgumentNullException("Config: Certificates/CurrentPath");

			currentCert = CertificateAndKey.FromPfx(currentPath, password);

			if (section.GetValue<string>("ArchivePath") is string archivePath)
			{
				foreach (string certPath in Directory.EnumerateFiles(archivePath))
					archive.Add(CertificateAndKey.FromPfx(certPath, password));
			}
		}


		private static CertificateAndKey FromPem(string directory, int? n = null)
		{
			string number = n?.ToString() ?? "";
			string certPath = Path.Combine(directory, pemCertName + number + ".pem");
			string keyPath = Path.Combine(directory, pemPrivkeyName + number + ".pem");

			if (!File.Exists(certPath) || !File.Exists(keyPath))
				return null;
			return CertificateAndKey.FromPem(certPath, keyPath);
		}
	}
}
