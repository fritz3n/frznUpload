using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client.Files
{
	class Screenshot : IFileProvider
	{
		private string Path;

		public List<UploadFile> GetFile(string format)
		{
			int screenLeft = SystemInformation.VirtualScreen.Left;
			int screenTop = SystemInformation.VirtualScreen.Top;
			int screenWidth = SystemInformation.VirtualScreen.Width;
			int screenHeight = SystemInformation.VirtualScreen.Height;

			var Screenshot = new Bitmap(screenWidth,
										   screenHeight,
										   PixelFormat.Format32bppArgb);

			using (var graphic = Graphics.FromImage(Screenshot))
			{

				graphic.CopyFromScreen(screenLeft,
											screenTop,
											0,
											0,
											Screenshot.Size,
											CopyPixelOperation.SourceCopy);
			}

			Path = TempFileHandler.RegisterFile();
			string filename = string.Format(format + ".Jpeg", DateTime.Now, "Screenshot");

			Screenshot.Save(Path, ImageFormat.Jpeg);

			return new List<UploadFile>{new UploadFile
			{
				Path = Path,
				Filename = filename,
			} };
		}

		public void FreeFile()
		{
			TempFileHandler.FreeFile(Path);
		}

		public bool IsAvailable()
		{
			return true;
		}
	}
}
