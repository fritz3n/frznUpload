﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using frznUpload.Web.Data;

namespace frznUpload.Web.Migrations
{
    [DbContext(typeof(Database))]
    [Migration("20210118205956_tokenName")]
    partial class tokenName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("frznUpload.Web.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Extension")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Filename")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("frznUpload.Web.Models.Share", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FileId")
                        .HasColumnType("int");

                    b.Property<bool>("FirstView")
                        .HasColumnType("bit");

                    b.Property<string>("FirstViewCookie")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastAccessed")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Public")
                        .HasColumnType("bit");

                    b.Property<bool>("PublicRegistered")
                        .HasColumnType("bit");

                    b.Property<string>("WhitelistText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Whitelisted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("Shares");
                });

            modelBuilder.Entity("frznUpload.Web.Models.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUsed")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Serial")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("frznUpload.Web.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Hash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TwoFaSecret")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("frznUpload.Web.Models.File", b =>
                {
                    b.HasOne("frznUpload.Web.Models.User", "User")
                        .WithMany("Files")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("frznUpload.Web.Models.Share", b =>
                {
                    b.HasOne("frznUpload.Web.Models.File", "File")
                        .WithMany("Shares")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("File");
                });

            modelBuilder.Entity("frznUpload.Web.Models.Token", b =>
                {
                    b.HasOne("frznUpload.Web.Models.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("frznUpload.Web.Models.File", b =>
                {
                    b.Navigation("Shares");
                });

            modelBuilder.Entity("frznUpload.Web.Models.User", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
