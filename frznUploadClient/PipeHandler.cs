using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace frznUpload.Client
{
	static class PipeHandler
	{
		public static bool IsServer { get; } = false;

		private static Mutex mutex = new Mutex(false, "frznUploadRunning");
		private static CancellationTokenSource tokenSource = new CancellationTokenSource();


		static PipeHandler()
		{
			Console.WriteLine("Checking if this is the first instance:");

			try
			{
				IsServer = mutex.WaitOne(TimeSpan.Zero, true);
			}
			catch (AbandonedMutexException)
			{
				IsServer = false;
				Console.WriteLine("Mutex abandoned");
			}

			Console.WriteLine("\t" + IsServer);
		}

		/// <summary>
		/// Release the Mutex and stop the NamedPipeServer
		/// </summary>
		public static void Close()
		{
			if (IsServer)
			{
				mutex.ReleaseMutex();
				tokenSource.Cancel();
			}
		}

		/// <summary>
		/// Sends arguments to the running server, if this isnt the server
		/// </summary>
		/// <param name="arguments">the arguments to be sent</param>
		public static void SendMessage(string[] arguments)
		{
			if (IsServer)
				return;

			var pipeClient =
					new NamedPipeClientStream(".", "frznUploadPipe",
						PipeDirection.Out, PipeOptions.Asynchronous);

			Console.WriteLine("Connecting...");

			pipeClient.Connect();

			var serializer = new XmlSerializer(typeof(string[]));

			var writer = new StreamWriter(pipeClient);

			using (var stringWriter = new StringWriter())
			{
				serializer.Serialize(stringWriter, arguments);

				writer.Write(stringWriter.ToString());
			}

			Console.WriteLine("Sent!");

			writer.Dispose();
			pipeClient.Dispose();
		}

		/// <summary>
		/// Starts the Named pipe server, if this is the server
		/// </summary>
		/// <param name="handler">An ArgumentsHandler to handle the incoming Messages from clients</param>
		public static void Start(ArgumentsHandler handler)
		{
			if (!IsServer)
				return;

			new Thread(() => PipeServer(handler, tokenSource.Token)).Start();
		}

		private static async void PipeServer(ArgumentsHandler handler, CancellationToken token)
		{
			var pipeServer = new NamedPipeServerStream("frznUploadPipe", PipeDirection.In, 1);

			var serializer = new XmlSerializer(typeof(string[]));


			while (true)
			{
				try
				{
					await pipeServer.WaitForConnectionAsync(token);
					var reader = new StreamReader(pipeServer, Encoding.UTF8, false, 100, true);

					string s = reader.ReadToEnd();
					Console.WriteLine("incoming Message:\n" + s);


					using (var stringReader = new StringReader(s))
					{
						handler.HandleArguments((string[])serializer.Deserialize(stringReader));
					}

					reader.Dispose();
					pipeServer.Disconnect();
				}
				catch (IOException e)
				{
					Console.WriteLine(e);
					pipeServer.Dispose();
					pipeServer = new NamedPipeServerStream("frznUploadPipe", PipeDirection.In, 1);
				}
			}
		}
	}
}
