using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client.Files
{
	class ClipboardProvider : IFileProvider
	{
		private readonly bool useText;
		private string Path = null;

		public ClipboardProvider(bool useText = false)
		{
			this.useText = useText;
		}

		public void FreeFile()
		{
			if (Path is not null)
				TempFileHandler.FreeFile(Path);
		}

		public List<UploadFile> GetFile(string format)
		{
			if (Clipboard.ContainsFileDropList())
			{
				var UploadList = new List<UploadFile>();

				System.Collections.Specialized.StringCollection FileList = Clipboard.GetFileDropList();

				foreach (string s in FileList)
				{
					var u = new UploadFile
					{
						Path = s,
						Filename = string.Format(format, DateTime.Now, System.IO.Path.GetFileName(s)),
					};
					UploadList.Add(u);
				}

				return UploadList;
			}

			if (Clipboard.ContainsImage())
			{
				Path = TempFileHandler.RegisterFile();
				string filename = string.Format(format + ".Jpeg", DateTime.Now, "Clipboard Image");

				Clipboard.GetImage().Save(Path, ImageFormat.Jpeg);

				return new List<UploadFile>{ new UploadFile
				{
					Path = Path,
					Filename = filename,
				}};
			}

			if (useText && Clipboard.ContainsText())
			{
				Path = TempFileHandler.RegisterFile();
				string filename = string.Format(format + ".txt", DateTime.Now, "Clipboard Text");

				File.WriteAllText(Path, Clipboard.GetText());

				return new List<UploadFile>{ new UploadFile
				{
					Path = Path,
					Filename = filename,
				}};
			}

			throw new InvalidOperationException("Please copy something before attmepting to upload from clipboard");
		}

		public bool IsAvailable()
		{
			return (useText && Clipboard.ContainsText()) || Clipboard.ContainsImage() || Clipboard.ContainsFileDropList();

		}
	}
}
