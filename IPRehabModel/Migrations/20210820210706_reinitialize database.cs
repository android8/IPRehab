using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IPRehab.Data.Migrations
{
    public partial class reinitializedatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblAnswer_tblCodeSet",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_app.tblEpisodeOfCare_app.tblPatient",
                schema: "app",
                table: "tblEpisodeOfCare");

            migrationBuilder.DropForeignKey(
                name: "FK_Answer_CodeSet",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_Form_CodeSet",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_Section_CodeSet",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_tblInstruction_tblQuestion",
                schema: "app",
                table: "tblQuestionInstruction");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_tblQuestion_FormFK",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropIndex(
                name: "IX_tblQuestion_FormSectionFK",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropIndex(
                name: "IX_tblQuestion_QuestionKey",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropIndex(
                name: "IX_tblPatient_UniqueName",
                schema: "app",
                table: "tblPatient");

            migrationBuilder.DropIndex(
                name: "IX_tblCodeSet",
                schema: "app",
                table: "tblCodeSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblAnswer",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropIndex(
                name: "IX_tblAnswer",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropColumn(
                name: "FormFK",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropColumn(
                name: "FormSectionFK",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.RenameColumn(
                name: "QuestionIdFK",
                schema: "app",
                table: "tblQuestionStage",
                newName: "QuestionIDFK");

            migrationBuilder.RenameColumn(
                name: "QuestionTitle",
                schema: "app",
                table: "tblQuestion",
                newName: "QuestionSection");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblUser",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NetworkName",
                schema: "app",
                table: "tblUser",
                type: "nchar(20)",
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblSignature",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<bool>(
                name: "Required",
                schema: "app",
                table: "tblQuestionStage",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                schema: "app",
                table: "tblQuestionStage",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblQuestionStage",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StageGroupTitle",
                schema: "app",
                table: "tblQuestionStage",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayLocation",
                schema: "app",
                table: "tblQuestionInstruction",
                type: "nchar(50)",
                fixedLength: true,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblQuestionInstruction",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StageCodeSetIDFK",
                schema: "app",
                table: "tblQuestionInstruction",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Question",
                schema: "app",
                table: "tblQuestion",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblQuestion",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblPatient",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblEpisodeOfCare",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblCodeSet",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "AnswerID",
                schema: "app",
                table: "tblAnswer",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "AnswerByUserID",
                schema: "app",
                table: "tblAnswer",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "app",
                table: "tblAnswer",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StageIDFK",
                schema: "app",
                table: "tblAnswer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblAnswer",
                schema: "app",
                table: "tblAnswer",
                column: "AnswerID");

            migrationBuilder.CreateIndex(
                name: "IX_tblUser_Name",
                schema: "app",
                table: "tblUser",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_tblUser_NetworkName",
                schema: "app",
                table: "tblUser",
                column: "NetworkName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblQuestionInstruction_StageCodeSetIDFK",
                schema: "app",
                table: "tblQuestionInstruction",
                column: "StageCodeSetIDFK");

            migrationBuilder.CreateIndex(
                name: "IX_tblQuestion",
                schema: "app",
                table: "tblQuestion",
                columns: new[] { "QuestionKey", "GroupTitle" },
                unique: true,
                filter: "[GroupTitle] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblPatient_UniqueName",
                schema: "app",
                table: "tblPatient",
                columns: new[] { "LastName", "FirstName", "MiddleName", "Last4SSN" },
                unique: true,
                filter: "[MiddleName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblCodeSet",
                schema: "app",
                table: "tblCodeSet",
                columns: new[] { "CodeSetParent", "CodeValue" },
                unique: true,
                filter: "[CodeSetParent] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblAnswer",
                schema: "app",
                table: "tblAnswer",
                columns: new[] { "EpsideOfCareIDFK", "QuestionIDFK", "AnswerCodeSetFK", "AnswerSequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblAnswer_StageIDFK",
                schema: "app",
                table: "tblAnswer",
                column: "StageIDFK");

            migrationBuilder.AddForeignKey(
                name: "FK_tblAnswer_tblCodeSet_AnswerCodeSet",
                schema: "app",
                table: "tblAnswer",
                column: "AnswerCodeSetFK",
                principalSchema: "app",
                principalTable: "tblCodeSet",
                principalColumn: "CodeSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tblAnswer_tblCodeSet_StageCodeSet",
                schema: "app",
                table: "tblAnswer",
                column: "StageIDFK",
                principalSchema: "app",
                principalTable: "tblCodeSet",
                principalColumn: "CodeSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tblEpisodeOfCare_tblPatient",
                schema: "app",
                table: "tblEpisodeOfCare",
                column: "PatientICNFK",
                principalSchema: "app",
                principalTable: "tblPatient",
                principalColumn: "ICN",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tblQuestion_tblCodeSet",
                schema: "app",
                table: "tblQuestion",
                column: "AnswerCodeSetFK",
                principalSchema: "app",
                principalTable: "tblCodeSet",
                principalColumn: "CodeSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tblQuestionInstruction_tblCodeSet",
                schema: "app",
                table: "tblQuestionInstruction",
                column: "StageCodeSetIDFK",
                principalSchema: "app",
                principalTable: "tblCodeSet",
                principalColumn: "CodeSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tblQuestionInstruction_tblQuestion",
                schema: "app",
                table: "tblQuestionInstruction",
                column: "QuestionIDFK",
                principalSchema: "app",
                principalTable: "tblQuestion",
                principalColumn: "QuestionID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblAnswer_tblCodeSet_AnswerCodeSet",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_tblAnswer_tblCodeSet_StageCodeSet",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_tblEpisodeOfCare_tblPatient",
                schema: "app",
                table: "tblEpisodeOfCare");

            migrationBuilder.DropForeignKey(
                name: "FK_tblQuestion_tblCodeSet",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_tblQuestionInstruction_tblCodeSet",
                schema: "app",
                table: "tblQuestionInstruction");

            migrationBuilder.DropForeignKey(
                name: "FK_tblQuestionInstruction_tblQuestion",
                schema: "app",
                table: "tblQuestionInstruction");

            migrationBuilder.DropIndex(
                name: "IX_tblUser_Name",
                schema: "app",
                table: "tblUser");

            migrationBuilder.DropIndex(
                name: "IX_tblUser_NetworkName",
                schema: "app",
                table: "tblUser");

            migrationBuilder.DropIndex(
                name: "IX_tblQuestionInstruction_StageCodeSetIDFK",
                schema: "app",
                table: "tblQuestionInstruction");

            migrationBuilder.DropIndex(
                name: "IX_tblQuestion",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropIndex(
                name: "IX_tblPatient_UniqueName",
                schema: "app",
                table: "tblPatient");

            migrationBuilder.DropIndex(
                name: "IX_tblCodeSet",
                schema: "app",
                table: "tblCodeSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblAnswer",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropIndex(
                name: "IX_tblAnswer",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropIndex(
                name: "IX_tblAnswer_StageIDFK",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblUser");

            migrationBuilder.DropColumn(
                name: "NetworkName",
                schema: "app",
                table: "tblUser");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblSignature");

            migrationBuilder.DropColumn(
                name: "Comment",
                schema: "app",
                table: "tblQuestionStage");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblQuestionStage");

            migrationBuilder.DropColumn(
                name: "StageGroupTitle",
                schema: "app",
                table: "tblQuestionStage");

            migrationBuilder.DropColumn(
                name: "DisplayLocation",
                schema: "app",
                table: "tblQuestionInstruction");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblQuestionInstruction");

            migrationBuilder.DropColumn(
                name: "StageCodeSetIDFK",
                schema: "app",
                table: "tblQuestionInstruction");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblQuestion");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblPatient");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblEpisodeOfCare");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblCodeSet");

            migrationBuilder.DropColumn(
                name: "AnswerID",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropColumn(
                name: "AnswerByUserID",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.DropColumn(
                name: "StageIDFK",
                schema: "app",
                table: "tblAnswer");

            migrationBuilder.RenameColumn(
                name: "QuestionIDFK",
                schema: "app",
                table: "tblQuestionStage",
                newName: "QuestionIdFK");

            migrationBuilder.RenameColumn(
                name: "QuestionSection",
                schema: "app",
                table: "tblQuestion",
                newName: "QuestionTitle");

            migrationBuilder.AlterColumn<bool>(
                name: "Required",
                schema: "app",
                table: "tblQuestionStage",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "Question",
                schema: "app",
                table: "tblQuestion",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldUnicode: false,
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<int>(
                name: "FormFK",
                schema: "app",
                table: "tblQuestion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FormSectionFK",
                schema: "app",
                table: "tblQuestion",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblAnswer",
                schema: "app",
                table: "tblAnswer",
                column: "EpsideOfCareIDFK");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(30)", nullable: true),
                    LastName = table.Column<string>(type: "varchar(30)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "app",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "app",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "app",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "app",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "app",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblQuestion_FormFK",
                schema: "app",
                table: "tblQuestion",
                column: "FormFK");

            migrationBuilder.CreateIndex(
                name: "IX_tblQuestion_FormSectionFK",
                schema: "app",
                table: "tblQuestion",
                column: "FormSectionFK");

            migrationBuilder.CreateIndex(
                name: "IX_tblQuestion_QuestionKey",
                schema: "app",
                table: "tblQuestion",
                column: "QuestionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPatient_UniqueName",
                schema: "app",
                table: "tblPatient",
                columns: new[] { "LastName", "FirstName", "MiddleName", "Last4SSN" },
                unique: true,
                filter: "[LastName] IS NOT NULL AND [MiddleName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblCodeSet",
                schema: "app",
                table: "tblCodeSet",
                columns: new[] { "CodeSetParent", "CodeValue" },
                unique: true,
                filter: "[CodeSetParent] IS NOT NULL AND [CodeValue] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblAnswer",
                schema: "app",
                table: "tblAnswer",
                columns: new[] { "EpsideOfCareIDFK", "QuestionIDFK", "AnswerCodeSetFK", "AnswerSequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "app",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "app",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "app",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "app",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "app",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "app",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "app",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_tblAnswer_tblCodeSet",
                schema: "app",
                table: "tblAnswer",
                column: "AnswerCodeSetFK",
                principalSchema: "app",
                principalTable: "tblCodeSet",
                principalColumn: "CodeSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_app.tblEpisodeOfCare_app.tblPatient",
                schema: "app",
                table: "tblEpisodeOfCare",
                column: "PatientICNFK",
                principalSchema: "app",
                principalTable: "tblPatient",
                principalColumn: "ICN",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_CodeSet",
                schema: "app",
                table: "tblQuestion",
                column: "AnswerCodeSetFK",
                principalSchema: "app",
                principalTable: "tblCodeSet",
                principalColumn: "CodeSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Form_CodeSet",
                schema: "app",
                table: "tblQuestion",
                column: "FormFK",
                principalSchema: "app",
                principalTable: "tblCodeSet",
                principalColumn: "CodeSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Section_CodeSet",
                schema: "app",
                table: "tblQuestion",
                column: "FormSectionFK",
                principalSchema: "app",
                principalTable: "tblCodeSet",
                principalColumn: "CodeSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tblInstruction_tblQuestion",
                schema: "app",
                table: "tblQuestionInstruction",
                column: "QuestionIDFK",
                principalSchema: "app",
                principalTable: "tblQuestion",
                principalColumn: "QuestionID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
