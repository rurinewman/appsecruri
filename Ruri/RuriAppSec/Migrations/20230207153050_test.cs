using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuriAppSec.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordDB",
                columns: table => new
                {
                    PasswordID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    newPassword = table.Column<string>(name: "new_Password", type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    userid = table.Column<string>(name: "user_id", type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordDB", x => x.PasswordID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordDB");
        }
    }
}
