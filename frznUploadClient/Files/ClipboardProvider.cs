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
        private string Path = null;

        public void FreeFile()
        {
            
        }

        public List<UploadFile> GetFile(string format)
        {
            if (Clipboard.ContainsFileDropList())
            {
                var UploadList = new List<UploadFile>();

                var FileList = Clipboard.GetFileDropList();

                foreach(string s in FileList)
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
                var filename = string.Format(format + ".Jpeg", DateTime.Now, "Clipboard Image");

                Clipboard.GetImage().Save(Path, ImageFormat.Jpeg);

                return new List<UploadFile>{ new UploadFile
                {
                    Path = Path,
                    Filename = filename,
                }};
            }

            if (Clipboard.ContainsText())
            {
                Path = TempFileHandler.RegisterFile();
                var filename = string.Format(format + ".txt", DateTime.Now, "Clipboard Text");

                File.WriteAllText(Path, Clipboard.GetText());

                return new List<UploadFile>{ new UploadFile
                {
                    Path = Path,
                    Filename = filename,
                }};
            }

            throw new InvalidOperationException();
        }
    }
}
