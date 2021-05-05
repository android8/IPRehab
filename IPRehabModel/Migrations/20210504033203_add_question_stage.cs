using Microsoft.EntityFrameworkCore.Migrations;

namespace IPRehab.Data.Migrations
{
    public partial class add_question_stage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblQuestionStage",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionIdFK = table.Column<int>(type: "int", nullable: false),
                    StageFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblQuestionStage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblQuestionStage_tblCodeSet",
                        column: x => x.StageFK,
                        principalSchema: "app",
                        principalTable: "tblCodeSet",
                        principalColumn: "CodeSetID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblQuestionStage_tblQuestion",
                        column: x => x.QuestionIdFK,
                        principalSchema: "app",
                        principalTable: "tblQuestion",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblQuestionStage",
                schema: "app",
                table: "tblQuestionStage",
                columns: new[] { "QuestionIdFK", "StageFK" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblQuestionStage_QuestionIdFk",
                schema: "app",
                table: "tblQuestionStage",
                column: "QuestionIdFK");

            migrationBuilder.CreateIndex(
                name: "IX_tblQuestionStage_StageFk",
                schema: "app",
                table: "tblQuestionStage",
                column: "StageFK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblQuestionStage",
                schema: "app");
        }
    }
}
