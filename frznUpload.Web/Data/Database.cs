
using frznUpload.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Data
{
	public class Database : DbContext
	{
		public Database(DbContextOptions<Database> options)
			: base(options)
		{ }

		public DbSet<User> Users { get; set; }
		public DbSet<Token> Tokens { get; set; }
		public DbSet<File> Files { get; set; }
		public DbSet<Share> Shares { get; set; }
		public DbSet<Visit> Visits { get; set; }


		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<User>().ToTable("Users").Property(e => e.Id);

			builder.Entity<User>().HasKey(u => u.Id);
			builder.Entity<User>().HasMany(u => u.Tokens).WithOne(t => t.User).OnDelete(DeleteBehavior.Cascade);


			builder.Entity<File>().ToTable("Files").Property(e => e.Id);

			builder.Entity<File>().HasKey(f => f.Id);
			builder.Entity<File>().HasMany(f => f.Shares).WithOne(s => s.File).OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Share>().ToTable("Shares").Property(e => e.Id);
			builder.Entity<Share>().HasKey(s => s.Id);
			builder.Entity<Share>().HasMany(s => s.Visits).WithOne(v => v.Share).OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Token>().ToTable("Tokens").Property(e => e.Id);
			builder.Entity<Token>().HasKey(t => t.Id);


			builder.Entity<Visit>().ToTable("Visits").Property(e => e.Id);
			builder.Entity<Visit>().HasKey(t => t.Id);
			builder.Entity<Visit>().HasOne(v => v.User).WithMany().OnDelete(DeleteBehavior.Cascade);

		}
	}
}
