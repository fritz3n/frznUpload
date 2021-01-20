using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client.Handlers
{
	static class CertificateHandler
	{

		const string password = "asdsadasasisijsdhaj8dz7s8788h";
		public static System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate { get; private set; } = null;
		private static RsaKeyParameters privateKey;

		private static SecureRandom random = new SecureRandom(new CryptoApiRandomGenerator());

		public static bool ContainsCertificate => Certificate != null;
		public static bool ShouldRenew => Certificate is null ? false : (DateTime.Parse(Certificate.GetExpirationDateString()) - DateTime.Now).TotalDays < 5;

		public static bool Load()
		{
			try
			{
				Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(Config.AppSettings["CertFile"].Value, password);
			}
			catch (Exception) { }
			return ContainsCertificate;
		}

		public static void Clear()
		{
			Certificate = null;
			File.Delete(Config.AppSettings["CertFile"].Value);
		}

		public static byte[][] GenerateKeyPair()
		{
			var keyGenerationParameters = new KeyGenerationParameters(random, 2048);
			var keyPairGenerator = new RsaKeyPairGenerator();
			keyPairGenerator.Init(keyGenerationParameters);
			AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();
			privateKey = keyPair.Private as RsaKeyParameters;
			var publicKey = keyPair.Public as RsaKeyParameters;

			byte[] exp = publicKey.Exponent.ToByteArray();
			byte[] mod = publicKey.Modulus.ToByteArray();

			return new[] { exp, mod };

		}

		public static void NewCertificate(byte[] certificateData)
		{

			//var cert = (Org.BouncyCastle.X509.X509Certificate)new Org.BouncyCastle.OpenSsl.PemReader(new StringReader(Encoding.UTF8.GetString(certificateData))).ReadObject();


			var store = new Pkcs12Store(new MemoryStream(certificateData), "".ToCharArray());
			//var certificateEntry = new X509CertificateEntry(cert);
			string friendlyName = store.Aliases.Cast<string>().First();
			X509CertificateEntry cert = store.GetCertificate(friendlyName);
			store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(privateKey), new[] { cert });

			var stream = new MemoryStream();
			store.Save(stream, password.ToCharArray(), random);

			Certificate = new X509Certificate2(stream.ToArray(), password, X509KeyStorageFlags.Exportable);
			byte[] data = Certificate.Export(X509ContentType.Pfx, password);
			File.WriteAllBytes(Config.AppSettings["CertFile"].Value, data);

			Certificate = new X509Certificate2(Config.AppSettings["CertFile"].Value, password);
		}
	}
}
