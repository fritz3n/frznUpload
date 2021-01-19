using frznUpload.Client.Handlers;
using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client
{
	public class Client : IDisposable
	{
		private TcpClient Tcp;
		private SslStream stream;
		private MessageHandler mes;

		public string Name { get; private set; }
		public bool IsAuthenticated { get; private set; }
		public bool Connected => Tcp.Connected;

		private bool disposed = false;

		public Client(string url, int port)
		{
			Connect(url, port);
		}

		public Client()
		{

		}

		private void OnDisconnect(object sender, MessageHandler.DisconnectReason disconnectReason)
		{
			Console.WriteLine("Disconnected: " + disconnectReason);
			Disconnect();
		}

		public void Connect(string url, int port, bool verbose = false)
		{
			ThrowIfDisposed();
			var stp = new Stopwatch();
			stp.Start();
			Tcp = new TcpClient
			{
				SendTimeout = 15000
			};

			Tcp.Connect(url, port);

			stream = new SslStream(Tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate));

			if (CertificateHandler.ContainsCertificate)
			{
				stream.AuthenticateAsClient(url, new X509Certificate2Collection() { CertificateHandler.Certificate }, true);

				mes = new MessageHandler(Tcp, stream, verbose);
				mes.Start();
				mes.OnDisconnect += OnDisconnect;

				Message message = mes.WaitForMessage(Message.MessageType.AuthSuccess);

				if (!message.IsError)
				{
					IsAuthenticated = true;
					Name = message[0];
				}

			}
			else
			{
				stream.AuthenticateAsClient(url);

				IsAuthenticated = false;
				mes = new MessageHandler(Tcp, stream, verbose);
				mes.Start();
				mes.OnDisconnect += OnDisconnect;
			}

			stp.Stop();

			Console.WriteLine("encryption established: " + stp.ElapsedMilliseconds);


			mes.SendMessage(new Message(Message.MessageType.Version, false, MessageHandler.Version));
			Message m = mes.WaitForMessage(true, Message.MessageType.Version);

			if (m[0] != MessageHandler.Version)
			{
				Dispose();
				throw new InvalidOperationException("Server version does not match Client version");
			}
		}

		public async Task ConnectAsync(string url, int port, bool verbose = false)
		{
			ThrowIfDisposed();
			var stp = new Stopwatch();
			stp.Start();
			Tcp = new TcpClient
			{
				SendTimeout = 15000
			};

			await Tcp.ConnectAsync(url, port);

			stream = new SslStream(Tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), new LocalCertificateSelectionCallback(Selection));


			stp.Stop();

			Console.WriteLine("encryption established: " + stp.ElapsedMilliseconds);


			if (CertificateHandler.ContainsCertificate)
			{
				await stream.AuthenticateAsClientAsync(url, new X509Certificate2Collection(new X509Certificate2[] { CertificateHandler.Certificate }), true);

				mes = new MessageHandler(Tcp, stream, verbose);
				mes.Start();
				mes.OnDisconnect += OnDisconnect;

				Message message = await mes.WaitForMessageAsync(Message.MessageType.AuthSuccess);

				if (!message.IsError)
				{
					IsAuthenticated = true;
					Name = message[0];
				}
			}
			else
			{
				await stream.AuthenticateAsClientAsync(url);

				IsAuthenticated = false;
				mes = new MessageHandler(Tcp, stream, verbose);
				mes.Start();
				mes.OnDisconnect += OnDisconnect;
			}

			mes.SendMessage(new Message(Message.MessageType.Version, false, MessageHandler.Version));
			Message m = await mes.WaitForMessageAsync(true, Message.MessageType.Version);

			if (m[0] != MessageHandler.Version)
			{
				Dispose();
				throw new InvalidOperationException("Server version does not match Client version");
			}
		}

		private X509Certificate Selection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			return localCertificates.Cast<X509Certificate>().FirstOrDefault();
		}

		public void Disconnect()
		{
			ThrowIfDisposed();
			mes?.Stop();
			Tcp?.Close();
		}

		public async Task RenewCert()
		{
			ThrowIfDisposed();

			string machineName = Environment.OSVersion + " - " + Environment.MachineName;

			byte[][] key = CertificateHandler.GenerateKeyPair();
			mes.SendMessage(new Message(Message.MessageType.CertRenewRequest, false, key[0], key[1], machineName));

			Message rec = await mes.WaitForMessageAsync(true, Message.MessageType.CertRenewSuccess);
			CertificateHandler.NewCertificate(rec[0]);
		}

		public async Task<bool> AuthWithPass(string username, string password)
		{
			ThrowIfDisposed();
			try
			{

				string machineName = Environment.OSVersion + " - " + Environment.MachineName;

				byte[][] key = CertificateHandler.GenerateKeyPair();
				mes.SendMessage(new Message(Message.MessageType.CertRequest, false, username, password, key[0], key[1], machineName));

				Message rec = await mes.WaitForMessageAsync(false);
				if (rec.IsError == true)
				{
					throw new Exception("Recived an error: " + rec.ToString());
				}
				else if (rec.Type == Message.MessageType.CertSuccess)
				{
					CertificateHandler.NewCertificate(rec[0]);
					return true;
				}
				else if (rec.Type == Message.MessageType.TwoFactorNeeded)
				{
					//Two fa needed
					await DoTwoFaExchangeAsync();
					return true;
				}
				else
				{
					//unexpected message
					throw new Exception("Unexpected message: " + rec.ToString());
				}
			}
			catch (SequenceBreakException e)
			{
				mes.SendMessage(new Message(Message.MessageType.Sequence, true));
				Console.WriteLine(e);
				return false;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return false;
			}
		}

		public async Task DeauthKey()
		{
			ThrowIfDisposed();
			try
			{
				mes.SendMessage(new Message(Message.MessageType.CertRevokeRequest));
				await mes.WaitForMessageAsync(true, Message.MessageType.CertRevokeSuccess);
				IsAuthenticated = false;
			}
			catch (SequenceBreakException e)
			{
				mes.SendMessage(new Message(Message.MessageType.Sequence, true));
				Console.WriteLine(e);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		public FileUploader UploadFile(string path, bool singleUse = false, string Filename = null)
		{
			ThrowIfDisposed();
			try
			{
				var up = new FileUploader(mes, this, path, singleUse, Filename);
				up.Start();
				return up;
			}
			catch (SequenceBreakException e)
			{
				mes.SendMessage(new Message(Message.MessageType.Sequence, true));
				Console.WriteLine(e);
				throw new AggregateException(e);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw new AggregateException(e);
			}
		}

		public async Task<string> ShareFile(string fileIdentifier, bool firstView = false, bool isPublic = true, bool publicRegistered = true, bool whitelisted = false, string whitelist = "")
		{
			ThrowIfDisposed();


			mes.SendMessage(new Message(Message.MessageType.ShareRequest, false, fileIdentifier, firstView ? 1 : 0, isPublic ? 1 : 0, publicRegistered ? 1 : 0, whitelisted ? 1 : 0, whitelist));

			Message m = await mes.WaitForMessageAsync(true, Message.MessageType.ShareResponse);

			return m[0];
		}

		public async Task<List<RemoteFile>> GetFiles()
		{
			ThrowIfDisposed();
			mes.SendMessage(new Message(Message.MessageType.FileListRequest));
			Message m = await mes.WaitForMessageAsync(true, Message.MessageType.FileList);

			var list = new List<RemoteFile>();

			for (int i = 1; i < m[0] + 1; i++)
			{
				Message fileInfo = m[i];

				var file = new RemoteFile
				{
					Filename = fileInfo[0],
					File_extension = fileInfo[1],
					Identifier = fileInfo[2],
					Size = fileInfo[3],
					Path = fileInfo[4]
				};

				list.Add(file);
			}

			return list;
		}

		public async Task<List<RemoteFile>> GetShares(string fileIdentifier)
		{
			ThrowIfDisposed();
			mes.SendMessage(new Message(Message.MessageType.FileListRequest));
			Message m = await mes.WaitForMessageAsync(true, Message.MessageType.FileList);

			var list = new List<RemoteFile>();

			for (int i = 1; i < m[0] + 1; i++)
			{
				Message fileInfo = m[i];

				var file = new RemoteFile
				{
					Filename = fileInfo[0],
					File_extension = fileInfo[1],
					Identifier = fileInfo[2],
					Size = fileInfo[3],
					Path = fileInfo[4]
				};

				list.Add(file);
			}

			return list;
		}
		public async Task DoTwoFaExchangeAsync()
		{
			try
			{
				mes.SendMessage(new Message(Message.MessageType.TwoFactorNeeded, false, GetTwoFaToken()));
				await mes.WaitForMessageAsync(true, Message.MessageType.TwoFactorSuccess);
			}
			catch (NoUserInputException)
			{
				mes.SendMessage(new Message(Message.MessageType.TwoFactorNeeded, true));
			}
		}

		public string GetTwoFaToken() => Prompt.ShowDialog("Pleas enter your Two factor authentication token", "Further authentication required");

		public async Task RemoveTwoFaAsync()
		{
			try
			{
				mes.SendMessage(new Message(Message.MessageType.TwoFactorRemove, false));
				await mes.WaitForMessageAsync(true, Message.MessageType.TwoFactorNeeded);
			}
			catch (Exception e)
			{
				throw new Exception("Error while requesting Two factor authentication removal", e);
			}
			try
			{
				await DoTwoFaExchangeAsync();
				await mes.WaitForMessageAsync(true, Message.MessageType.TwoFactorRemove);
			}
			catch (Exception e)
			{
				throw new Exception("Error while deleting Two factor authentication", e);
			}
		}

		public async Task<bool> GetHasTwoFaEnabled()
		{
			mes.SendMessage(new Message(Message.MessageType.HasTwoFactor, false));
			Message m = await mes.WaitForMessageAsync(Message.MessageType.HasTwoFactor);
			if (m.IsError)
			{
				throw new Exception("Error getting Two factor authentication status.");
			}
			else
			{
				return m.Fields[0] == 1;
			}
		}

		public async Task<string> GetTwoFaSecret()
		{
			mes.SendMessage(new Message(Message.MessageType.TwoFactorAdd));
			Message m = await mes.WaitForMessageAsync(true, Message.MessageType.TwoFactorAdd);
			return m.Fields[0];
		}

		/// <summary>
		/// Asks the server to delete a file
		/// </summary>
		public async Task DeleteFileAsync(string file_identifier)
		{
			mes.SendMessage(new Message(Message.MessageType.DeleteFile, false, file_identifier));
			await mes.WaitForMessageAsync(true, Message.MessageType.DeleteFile);
		}


		private static bool ValidateServerCertificate(
			  object sender,
			  X509Certificate certificate,
			  X509Chain chain,
			  SslPolicyErrors sslPolicyErrors)
		{


			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;

			Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

#if DEBUG
			Console.WriteLine("Ignoring due to debug mode");
			return true;
#endif

			// Do not allow this client to communicate with unauthenticated servers.
			return false;
		}

		private void ThrowIfDisposed()
		{
			if (disposed)
				throw new ObjectDisposedException("Client");
		}

		public void Dispose()
		{
			if (disposed)
				return;
			Disconnect();
			disposed = true;
			Tcp?.Dispose();
			stream?.Dispose();
			mes?.Dispose();
			GC.SuppressFinalize(this);
		}

		~Client()
		{
			Dispose();
		}
	}
}
