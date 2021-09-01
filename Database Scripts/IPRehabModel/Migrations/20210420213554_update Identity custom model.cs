using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IPRehab.Data.Migrations
{
    public partial class updateIdentitycustommodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");


            migrationBuilder.AlterDatabase(
                collation: "SQL_Latin1_General_CP1_CI_AS");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "app",
                table: "AspNetUsers",
                type: "varchar(30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "app",
                table: "AspNetUsers",
                type: "varchar(30)",
                nullable: true);


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "app",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "app",
                table: "AspNetUsers");


            migrationBuilder.AlterDatabase(
                oldCollation: "SQL_Latin1_General_CP1_CI_AS");
        }
    }
}
