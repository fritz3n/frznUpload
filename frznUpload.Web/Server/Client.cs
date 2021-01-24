using Castle.Core.Logging;
using frznUpload.Shared;
using frznUpload.Web.Data;
using frznUpload.Web.Models;
using frznUpload.Web.Server.Certificates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Web.Server
{
	class Client : IDisposable
	{
		private readonly TcpClient Tcp;
		private SslStream stream;
		private MessageHandler mes;
		private CancellationTokenSource tokenSource;
		private DatabaseHandler db;
		private ILogger<Client> log;
		private readonly CertificateHandler certificateHandler;
		private VerificationResult verificationResult;
		private string verificationSerial;

		public bool IsAuthenticated { get; private set; }

		public event EventHandler OnDispose;

		public Client(TcpClient tcp, DatabaseHandler db, ILogger<Client> log, CertificateHandler handler, bool verbose)
		{
			log.LogInformation("Client born");

			Tcp = tcp;
			this.db = db;
			this.log = log;
			certificateHandler = handler;
			stream = new SslStream(tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), new LocalCertificateSelectionCallback(Selection));

			stream.AuthenticateAsServer(handler.Certificate, true, false);

			mes = new MessageHandler(tcp, stream, verbose, new MessageLogger(log));
			mes.OnDisconnect += OnDisconnect;
			mes.Start();

			if (verificationResult == VerificationResult.Valid || verificationResult == VerificationResult.ValidArchival)
			{
				try
				{
					db.SetUser(verificationSerial);
					IsAuthenticated = true;
					mes.SendMessage(new Message(Message.MessageType.AuthSuccess, false, db.Name));
				}
				catch (KeyNotFoundException)
				{
					mes.SendMessage(new Message(Message.MessageType.AuthSuccess, true));
				}
			}


			log.LogDebug("Encryption established");


			log.LogDebug("Client initialized");
		}
		private bool ValidateServerCertificate(
			  object sender,
			  X509Certificate certificate,
			  X509Chain chain,
			  SslPolicyErrors sslPolicyErrors)
		{

			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
			{
				verificationResult = VerificationResult.None;
				return true;
			}

			verificationResult = certificateHandler.Verify(certificate);

			if (verificationResult == VerificationResult.Invalid)
				return false;

			verificationSerial = certificate.GetSerialNumberString();

			return true;
		}

		private X509Certificate Selection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate? remoteCertificate, string[] acceptableIssuers)
		{
			return localCertificates.Cast<X509Certificate>().First();
		}

		private void OnDisconnect(object sender, MessageHandler.DisconnectReason disconnectReason) => log.LogInformation("Disconnected: " + disconnectReason);

		public void Start()
		{
			tokenSource?.Cancel();
			tokenSource = new CancellationTokenSource();

			mes.SendMessage(new Message(Message.MessageType.Version, false, MessageHandler.Version));
			Message m = mes.WaitForMessage(true, Message.MessageType.Version);

			log.LogDebug("Client Version: " + m[0] as string);

			if (m[0] != MessageHandler.Version)
			{
				log.LogWarning("Client Version does not match Server version: " + m[0] as string);
				throw new InvalidOperationException("Server version does not match Client version");
			}

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			new Thread(() => ClientLoop(tokenSource.Token)).Start();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		}

		public void Stop()
		{
			tokenSource?.Cancel();
			mes?.Stop();
		}

		private async Task ClientLoop(CancellationToken token)
		{
			try
			{

				while (true)
				{
					Message message;
					try
					{
						var t = new Task<Message>(() => mes.WaitForMessage(), token);
						t.Start();

						message = await t;
					}
					catch (GracefulShutdownException)
					{
						log.LogInformation("Client disconnected");

						Dispose();
						return;
					}
					catch (Exception e)
					{
						log.LogError(e, "Error during receive, shutting down");
						try
						{
							mes.SendMessage(new Message(Message.MessageType.None, true, e.ToString()));
						}
						catch { }

						Dispose();
						return;
					}



					if (message.IsError)
					{
						log.LogInformation("Encountered a remote error:\n" + message);
						stream.Close();
					}

					#region user is not authed
					if (!IsAuthenticated)
					{
						switch (message.Type)
						{
							case Message.MessageType.CertRequest:

								if (DoTwoFaCheckIfNeeded(message[0]))
								{
									if (db.IsValid(message[0], message[1]))
									{
										CertificationResult result = certificateHandler.Certify(new[] { message[2] as byte[], message[3] as byte[] });
										db.SetSerial(message[0], result.SerialNumber, result.ValidUntil, message[4]);

										mes.SendMessage(new Message(Message.MessageType.CertSuccess, false, result.Rawdata));
										log.LogDebug("Authenticated with a Public Key");
										break;

									}
								}
								mes.SendMessage(new Message(Message.MessageType.CertSuccess, true, "Login data not correct"));
								log.LogInformation("Failed to authenticate a Public Key");

								break;

							default:
								mes.SendMessage(new Message(Message.MessageType.Sequence, true, "Not authenticated"));
								break;
						}
					}
					#endregion
					else
					{
						switch (message.Type)
						{
							case Message.MessageType.CertRevokeRequest:
								db.Deauthenticate();
								IsAuthenticated = false;
								mes.SendMessage(new Message(Message.MessageType.CertRevokeSuccess));

								log.LogInformation("Deauthenticated");

								break;


							case Message.MessageType.CertRenewRequest:
								CertificationResult result = certificateHandler.Certify(new[] { message[2] as byte[], message[3] as byte[] });
								db.SetSerial(message[0], result.SerialNumber, result.ValidUntil, message[4]);
								db.RemoveSerial(db.SerialNumber);

								mes.SendMessage(new Message(Message.MessageType.CertRenewSuccess, false, result.Rawdata));

								log.LogInformation("Certificate renewed");

								break;

							case Message.MessageType.FileUploadRequest:

								(bool, string) returned = await FileHandler.ReceiveFile(message, mes, db);

								if (returned.Item1)
									log.LogInformation("Uploaded a file: ", returned.Item2.Substring(0, 10));
								else
									log.LogInformation("Failed to upload a file");

								break;

							case Message.MessageType.FileListRequest:

								List<File> fileList = db.GetFiles();

								var Fields = new dynamic[1 + fileList.Count];
								Fields[0] = fileList.Count;

								for (int i = 0; i < fileList.Count; i++)
								{
									Fields[i + 1] = new Message(Message.MessageType.FileInfo, false, fileList[i].Filename ?? "", fileList[i].Extension ?? "", fileList[i].Identifier, fileList[i].Size, fileList[i].Path ?? "");
								}

								mes.SendMessage(new Message(Message.MessageType.FileList, false, Fields));

								log.LogDebug("Listed files");

								break;

							case Message.MessageType.ShareListRequest:

								List<Share> shareList = db.GetShares(message[0]);

								Fields = new dynamic[1 + shareList.Count];
								Fields[0] = shareList.Count;

								for (int i = 0; i < shareList.Count; i++)
								{
									Fields[i + 1] = new Message(Message.MessageType.FileInfo, false, shareList[i].Identifier, shareList[i].File.Identifier, shareList[i].FirstView, shareList[i].Public, shareList[i].PublicRegistered, shareList[i].Whitelisted, shareList[i].WhitelistText);
								}

								mes.SendMessage(new Message(Message.MessageType.FileList, false, Fields));

								log.LogDebug("Listed shares for file: " + message[0] as string);

								break;

							case Message.MessageType.ShareRequest:

								string id = db.SetFileShare(
									message[0],
									message[1] == 1,
									message[2] == 1,
									message[3] == 1,
									message[4] == 1,
									(message[5] as string).Split(';', StringSplitOptions.RemoveEmptyEntries)
									);

								mes.SendMessage(new Message(Message.MessageType.ShareResponse, false, id));

								log.LogInformation("Created a share: " + id);

								break;
							case Message.MessageType.DeleteFile:
								string file_identifier = message.Fields[0];
								try
								{
									//delete the file from the fs
									FileHandler.DeleteFile(file_identifier);
									//delete all database records of it
									db.DeleteFile(file_identifier);
									mes.SendMessage(new Message(Message.MessageType.DeleteFile, false, ""));
									log.LogInformation("Deleted file: " + file_identifier.Substring(0, 10));
								}
								catch (Exception e)
								{
									if (e.GetType() != typeof(UnauthorizedAccessException) && e.GetType() != typeof(ArgumentException))
									{
										log.LogError(e, "Error while deleting file");
									}
									log.LogWarning("Failed to delete file: " + file_identifier.Substring(0, 10));
									mes.SendMessage(new Message(Message.MessageType.DeleteFile, true, "Error deleting"));
								}
								break;
							#region TwoFa
							case Message.MessageType.TwoFactorAdd:
								if (!db.HasTwoFa())
								{
									string secret = TwoFactorHandler.CreateSecret();
									mes.SendMessage(new Message(Message.MessageType.TwoFactorAdd, false, TwoFactorHandler.GenerateQrCode(db.Name, secret)));
									db.SetTwoFactorSecret(secret);
									log.LogInformation("Added Two fa");
								}
								else
								{
									mes.SendMessage(new Message(Message.MessageType.TwoFactorAdd, true));
									log.LogInformation("Two Fa already exists");
								}

								break;
							case Message.MessageType.TwoFactorRemove:
								try
								{
									if (DoTwoFaCheckIfNeeded())
									{
										db.RemoveTwoFactorSecret();
										mes.SendMessage(new Message(Message.MessageType.TwoFactorRemove, false));
									}
								}
								catch (UnauthorizedAccessException e)
								{
									log.LogError(e, "Error while removing Two Fa");
									Dispose();
								}

								break;
							case Message.MessageType.HasTwoFactor:
								try
								{
									if (db.HasTwoFa())
									{
										//has secret
										mes.SendMessage(new Message(Message.MessageType.HasTwoFactor, false, 1));
									}
									else
									{
										//no secret
										mes.SendMessage(new Message(Message.MessageType.HasTwoFactor, false, 0));
									}
								}
								catch (Exception e)
								{
									log.LogError(e, "Error while testing if user has TwoFa");
									mes.SendMessage(new Message(Message.MessageType.HasTwoFactor, true));
								}
								break;
							#endregion
							default:
								mes.SendMessage(new Message(Message.MessageType.Sequence, true, "Not expected"));
								break;
						}
					}
				}
			}
			catch (SequenceBreakException e)
			{
				log.LogError(e, "Sequence break, Shutting down client");
				try
				{
					mes.SendMessage(new Message(Message.MessageType.Sequence, true, e.ToString()));
				}
				catch { }
				Dispose();
			}
			catch (Exception e)
			{
				log.LogError(e, "Shutting down client");
				try
				{
					mes.SendMessage(new Message(Message.MessageType.None, true, e.ToString()));
				}
				catch { }

				Dispose();
			}
		}


		private bool DoTwoFaCheckIfNeeded(string username = null)
		{
			//check if the user has TwoFa enabled, if yes -> send him that we need proof!
			int? id = null;

			if (username != null)
			{
				id = db.GetUserId(username);

				if (id == null)
					return true;
			}

			if (db.HasTwoFa(id))
			{
				string secret = db.GetTwoFactorSecret(id);
				mes.SendMessage(new Message(Message.MessageType.TwoFactorNeeded, false, ""));
				Message twoFaMessage = mes.WaitForMessage(false, Message.MessageType.TwoFactorNeeded);
				if (twoFaMessage.IsError == true)
				{
					return false; //client send error -> user did not enter a valid thingy
				}
				//if he cant prove who he is -> throw him out
				if (!TwoFactorHandler.Verify(secret, twoFaMessage.Fields[0]))
				{
					throw new UnauthorizedAccessException("TwoFa failed!");
				}
				else
				{
					mes.SendMessage(new Message(Message.MessageType.TwoFactorSuccess, false));
					return true;
				}
			}
			else
			{
				return true;
			}
		}

		public void Dispose()
		{
			Stop();
			stream?.Dispose();
			stream = null;

			mes?.Dispose();
			mes = null;

			OnDispose?.Invoke(this, null);
		}

		~Client() => Dispose();
	}
}
