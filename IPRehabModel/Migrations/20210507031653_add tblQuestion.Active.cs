using Microsoft.EntityFrameworkCore.Migrations;

namespace IPRehab.Data.Migrations
{
    public partial class addtblQuestionActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "tblQuestion",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "tblQuestion");
        }
    }
}
