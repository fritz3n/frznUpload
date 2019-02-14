﻿using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;

namespace frznUpload.Server
{
    class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string Tow_fa_secret { get; set; }
    }

    class Token
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public byte[] Signature { get; set; }
        public DateTime Created { get; set; }
        public DateTime Last_used { get; set; }
    }

    class Sql_File
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Identifier { get; set; }
        public string Filename { get; set; }
        public string File_extension { get; set; }
        public int Size { get; set; }
        public long Tags { get; set; }
    }

    class Share
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
