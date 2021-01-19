using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client
{
	public class RemoteFile
	{
		public string Identifier { get; set; }
		public string Filename { get; set; }
		public string File_extension { get; set; }
		public int Size { get; set; }
		public string SizeString => BytesToString(Size);
		public string Path { get; set; }

		public override string ToString()
		{
			return $"{Identifier.Substring(0, 10)}: {Filename}.{File_extension} ; {BytesToString(Size)}";
		}

		static string BytesToString(long byteCount)
		{
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
			if (byteCount == 0)
				return "0" + suf[0];
			long bytes = Math.Abs(byteCount);
			int place = (int)Math.Floor(Math.Log(bytes, 1024));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}
	}

	class RemoteShare
	{
		public bool Shared { get; private set; } = false;
		public string ShareIdentifier { get; set; }
		public string FileIdentifier { get; set; }
		public bool FirstView { get; set; }
		public bool Public { get; set; }
		public bool PublicToRegistered { get; set; }
		public bool Whitelisted { get; set; }
		public string Whitelist { get; set; }

		public RemoteShare(Message message)
		{
			ShareIdentifier = message[0];
			FileIdentifier = message[1];
			FirstView = message[2] != 0;
			Public = message[3] != 0;
			PublicToRegistered = message[4] != 0;
			Whitelisted = message[5] != 0;
			Whitelist = message[6];
		}

		public RemoteShare(string fileIdentifier, bool firstView = false, bool isPublic = true, bool publicRegistered = true, bool whitelisted = false, string whitelist = "")
		{
			FileIdentifier = fileIdentifier;
			FirstView = firstView;
			Public = isPublic;
			PublicToRegistered = publicRegistered;
			Whitelisted = whitelisted;
			Whitelist = whitelist;
		}

		public Message GetMessage()
		{
			return new Message(Message.MessageType.ShareInfo, false,
				ShareIdentifier,
				FileIdentifier,
				FirstView ? 1 : 0,
				Public ? 1 : 0,
				PublicToRegistered ? 1 : 0,
				Whitelisted ? 1 : 0,
				Whitelist
				);
		}
	}
}