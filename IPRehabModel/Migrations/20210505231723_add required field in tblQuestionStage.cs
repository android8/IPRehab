using Microsoft.EntityFrameworkCore.Migrations;

namespace IPRehab.Data.Migrations
{
    public partial class addrequiredfieldintblQuestionStage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Required",
                schema: "app",
                table: "tblQuestionStage",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Required",
                schema: "app",
                table: "tblQuestionStage");
        }
    }
}
