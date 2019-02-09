using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client.Files
{
    class ScreenClip : IFileProvider
    {
        private string Path;

        public void FreeFile()
        {
            TempFileHandler.FreeFile(Path);
        }

        public List<UploadFile> GetFile(string format)
        {
            Process proc = Process.Start("snippingtool", "/clip");

            proc.WaitForExit();

            if (!Clipboard.ContainsImage())
                throw new InvalidOperationException();
            
            Path = TempFileHandler.RegisterFile();
            var filename = string.Format(format + ".Jpeg", DateTime.Now);

            Clipboard.GetImage().Save(Path, ImageFormat.Jpeg);

            return new List<UploadFile>{ new UploadFile
            {
                Path = Path,
                Filename = filename,
            }};
        }
    }
}
