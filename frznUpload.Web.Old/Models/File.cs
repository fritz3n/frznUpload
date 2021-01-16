using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Models
{
    public class File
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Identifier { get; set; }
        public string Filename { get; set; }
        public string File_extension { get; set; }
        public int Size { get; set; }
        public long Tags { get; set; }
    }
}
