﻿using frznUpload.Shared;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Client
{
	public class FileUploader : IDisposable
	{
		private ILog log = LogManager.GetLogger(nameof(FileUploader));

		int ChunkSize = 5476; // Fits perfectly into 4 TCP segments

		public int TotalSize { get; private set; }
		public int WrittenSize { get; private set; }
		public double Progress
		{
			get
			{
				double v = (double)WrittenSize / TotalSize;
				if (v == double.NaN)
					return 0;
				return v;
			}
		}
		public bool Finished { get; private set; }
		public bool Running { get; private set; }
		public string Identifier { get; private set; }

		public bool Error { get; private set; } = false;
		public Exception Exception { get; private set; }

		public string FilePath { get; private set; }

		private CancellationTokenSource tokenSource = new CancellationTokenSource();
		private FileStream file;

		MessageHandler mes;
		Client client;
		bool SingleUse;

		public string Filename { get; private set; }

		public FileUploader(MessageHandler messageHandler, Client client, string path, bool singleUse = false, string filename = null)
		{
			mes = messageHandler;
			FilePath = path;
			this.client = client;
			SingleUse = singleUse;
			Filename = filename;
		}

		public void Start()
		{
			if (Running)
				throw new Exception();
			Running = true;

			tokenSource = new CancellationTokenSource();

			new Task(UploadLoop, tokenSource.Token).Start();
		}

		private async void UploadLoop()
		{
			try
			{
				Open();
				var info = new FileInfo(FilePath);
				int size = (int)(info.Length > int.MaxValue ? throw new FileLoadException() : info.Length);

				if (Filename == null)
					Filename = info.Name;

				string filename = Path.GetFileNameWithoutExtension(Filename);
				string extension = Path.GetExtension(Filename).Replace(".", "");
				string path = Path.GetDirectoryName(Filename).Replace('\\', '/');

				if (!path.StartsWith('/'))
					path = '/' + path;
				if (!path.EndsWith('/'))
					path += '/';

				TotalSize = size;

				mes.SendMessage(new Message(Message.MessageType.FileUploadRequest, false, filename, extension, "/", size));

				Message m = mes.WaitForMessage(true, Message.MessageType.FileUploadApproved);

				//mes.Stop();

				string identifier = m[0];
				byte[] buffer = new byte[ChunkSize];
				int written = 0;

				while ((written = await file.ReadAsync(buffer, 0, ChunkSize)) > 0)
				{
					mes.SendMessage(new Message(Message.MessageType.FileUpload, false, written, buffer.Clone()));
					mes.Flush();
					WrittenSize += written;
				}

				mes.SendMessage(new Message(Message.MessageType.FileUploadFinished, false));

				//mes.Start();

				mes.WaitForMessage(true, Message.MessageType.FileUploadSuccess);

				Identifier = identifier;
			}
			catch (SequenceBreakException e)
			{
				try
				{
					mes.SendMessage(new Message(Message.MessageType.Sequence, true));
				}
				catch { }
				Close();
				log.Info(e);
				throw new AggregateException(e);
			}
			catch (Exception e)
			{
				try
				{
					mes.SendMessage(new Message(Message.MessageType.None, true, e.ToString()));
				}
				catch { }
				Close();
				log.Info(e);
				Exception = e;
				Error = true;
			}

			Close();

			Finished = true;
			Running = false;
		}

		public void Abort()
		{
			tokenSource.Cancel();
			Running = false;
		}

		private void Open()
		{
			file = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}

		private void Close()
		{
			if (SingleUse)
			{
				client?.Dispose();
			}
			file?.Close();
			file?.Dispose();
		}

		public void Dispose()
		{
			Close();
			tokenSource.Dispose();
			GC.SuppressFinalize(this);
		}

		~FileUploader()
		{
			Dispose();
		}
	}

	public struct UploadContract
	{
		public FileUploader Uploader { get; set; }
		public string Path { get; set; }
		public string Filename { get; set; }

		public bool IsSharing { get; set; }

		public bool Share { get; set; }
		public bool FirstView { get; set; }
		public bool Public { get; set; }
		public bool PublicRegistered { get; set; }
		public bool Whitelisted { get; set; }
		public string Whitelist { get; set; }
	}
}
