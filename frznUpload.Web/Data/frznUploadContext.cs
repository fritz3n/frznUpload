
using frznUpload.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Data
{
    public class frznUploadContext : DbContext
    {
        public frznUploadContext(DbContextOptions<frznUploadContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Share> Shares { get; set; }
    }
}
