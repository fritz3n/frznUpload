using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace frznUpload.Web.Migrations
{
    public partial class Serial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Tokens");

            migrationBuilder.RenameColumn(
                name: "Last_used",
                table: "Tokens",
                newName: "ValidUntil");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsed",
                table: "Tokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Serial",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUsed",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "Serial",
                table: "Tokens");

            migrationBuilder.RenameColumn(
                name: "ValidUntil",
                table: "Tokens",
                newName: "Last_used");

            migrationBuilder.AddColumn<byte[]>(
                name: "Signature",
                table: "Tokens",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
