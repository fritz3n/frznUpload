using frznUpload.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Web.Server
{
	static class FileHandler
	{
		static string directory;
		const int chunksSize = 16384;


		public static void Init(IConfiguration config)
		{
			directory = config.GetValue<string>("FileDirectory");

			//Check if dir exsist, if not create it
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
		}

		public static async Task<(bool, string)> ReceiveFile(Message message, MessageHandler mes, DatabaseHandler db)
		{
			int size = message[3];
			int written = 0;

			string filename = message[0];
			string extension = message[1];
			string path = message[2];

			string identifier = db.GetAvailableFileIdentifier();

			mes.SendMessage(new Message(Message.MessageType.FileUploadApproved, false, identifier));



			string localFilename = Path.Combine(directory, identifier + ".file");

			FileStream file = File.Open(localFilename, FileMode.Create, FileAccess.Write);

			bool error = false;

			Message m;
			while (true)
			{
				try
				{
					m = mes.WaitForMessage(true);
				}
				catch
				{
					file.Close();
					file.Dispose();

					File.Delete(localFilename);

					throw;
				}

				if (m.Type == Message.MessageType.FileUploadFinished)
				{
					if (written != size)
					{
						mes.SendMessage(new Message(Message.MessageType.FileUpload, true, $"Expected {size} bytes, got {written}"));
						error = true;
						break;
					}

					mes.SendMessage(new Message(Message.MessageType.FileUploadSuccess, false));
					break;
				}
				else if (m.Type != Message.MessageType.FileUpload)
				{
					mes.SendMessage(new Message(Message.MessageType.Sequence, true));
					error = true;
					break;
				}



				if (written >= size)
				{
					mes.SendMessage(new Message(Message.MessageType.FileUpload, true, $"Expected {size} bytes, got {written}"));
					error = true;
					break;
				}

				try
				{
					await file.WriteAsync(m[1], 0, m[0]);
					written += m[0];
				}
				catch (Exception e)
				{
					mes.SendMessage(new Message(Message.MessageType.FileUpload, true, e.ToString()));
					error = true;
					break;
				}
			}

			file.Close();
			file.Dispose();

			if (error)
			{
				File.Delete(localFilename);
			}
			else
			{
				db.CreateFile(identifier, filename, extension, path, size);
			}

			return (!error, identifier);
		}

		public static void DeleteFile(string identifier)
		{
			string localFileName = Path.Combine(directory, identifier + ".file");
			//Delete the file from the fs
			File.Delete(localFileName);
		}
	}
}
