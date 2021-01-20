using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace frznUpload.Web.Server.Certificates
{
	public class CertificateAndKey
	{
		private Org.BouncyCastle.X509.X509Certificate certificate;
		private AsymmetricKeyParameter privateKey;

		public System.Security.Cryptography.X509Certificates.X509Certificate2 X509Certificate { get; private set; }

		public void LoadPfx(string path, string password)
		{
			var store = new Pkcs12Store(new FileStream(path, FileMode.Open, FileAccess.Read), password.ToCharArray());
			string alias = store.Aliases.Cast<string>().First();

			certificate = store.GetCertificate(alias).Certificate;
			privateKey = store.GetKey(alias).Key;

			X509Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(path, password);
		}

		public void LoadPem(string certPath, string keyPath)
		{
			using (StreamReader reader = File.OpenText(certPath))
				certificate = (Org.BouncyCastle.X509.X509Certificate)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
			using (StreamReader reader = File.OpenText(keyPath))
				privateKey = (AsymmetricKeyParameter)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();

			Pkcs12Store store = new Pkcs12StoreBuilder().Build();
			var certificateEntry = new X509CertificateEntry(certificate);
			string friendlyName = certificate.SubjectDN.ToString();
			store.SetCertificateEntry(friendlyName, certificateEntry);
			store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(privateKey), new[] { certificateEntry });


			var random = new SecureRandom(new CryptoApiRandomGenerator());
			var stream = new MemoryStream();
			store.Save(stream, "".ToCharArray(), random);
			X509Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(stream.ToArray(), "");

		}

		public ISignatureFactory GetFactory(SecureRandom random)
		{
			return new Asn1SignatureFactory("SHA256WITHRSA", privateKey, random);
		}

		public bool Verify(System.Security.Cryptography.X509Certificates.X509Certificate subject)
		{
			Org.BouncyCastle.X509.X509Certificate subjectCert = DotNetUtilities.FromX509Certificate(subject);
			return Verify(subjectCert);
		}

		public bool Verify(Org.BouncyCastle.X509.X509Certificate subject)
		{
			try
			{
				subject.Verify(certificate.GetPublicKey());
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static CertificateAndKey FromPem(string certPath, string keyPath)
		{
			var cert = new CertificateAndKey();
			cert.LoadPem(certPath, keyPath);
			return cert;
		}
		public static CertificateAndKey FromPfx(string path, string password)
		{
			var cert = new CertificateAndKey();
			cert.LoadPfx(path, password);
			return cert;
		}
	}
}
