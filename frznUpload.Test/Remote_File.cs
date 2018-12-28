using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Test
{
    class Remote_File
    {
        public string Identifier { get; set; }
        public string Filename { get; set; }
        public string File_extension { get; set; }
        public int Size { get; set; }
        public long Tags { get; set; }

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
}
