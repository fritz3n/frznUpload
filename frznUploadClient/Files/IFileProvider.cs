using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client.Files
{
    interface IFileProvider
    {
        List<UploadFile> GetFile(string format);
        void FreeFile();
    }

    public struct UploadFile
    {
        public string Path { get; set; }
        public string Filename { get; set; }
    }
}
