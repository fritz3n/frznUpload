using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Models
{
    public class Share
    {
        public int Id { get; set; }

        public string File_identifier { get; set; }
        public string Share_id { get; set; }

        public bool First_view { get; set; }
        public string First_view_cookie { get; set; }

        public bool Public { get; set; }
        public bool Public_registered { get; set; }

        public bool Whitelisted { get; set; }
        public string Whitelist { get; set; }
    }
}
