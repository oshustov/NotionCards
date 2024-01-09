using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotionCards.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LearningPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotionDbRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Translation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotionDbRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningPlanQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearningPlanId = table.Column<int>(type: "int", nullable: false),
                    ProgressId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPlanQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningPlanQuestion_LearningPlans_LearningPlanId",
                        column: x => x.LearningPlanId,
                        principalTable: "LearningPlans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearnPlanQuestionId = table.Column<int>(type: "int", nullable: false),
                    NotionDbRecordId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_LearningPlanQuestion_LearnPlanQuestionId",
                        column: x => x.LearnPlanQuestionId,
                        principalTable: "LearningPlanQuestion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_NotionDbRecords_NotionDbRecordId",
                        column: x => x.NotionDbRecordId,
                        principalTable: "NotionDbRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Progress",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progress", x => new { x.AnswerId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_Progress_LearningPlanQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "LearningPlanQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Progress_QuestionAnswer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "QuestionAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningPlanQuestion_LearningPlanId",
                table: "LearningPlanQuestion",
                column: "LearningPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NotionDbRecords_NotionId",
                table: "NotionDbRecords",
                column: "NotionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Progress_QuestionId",
                table: "Progress",
                column: "QuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_LearnPlanQuestionId",
                table: "QuestionAnswer",
                column: "LearnPlanQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_NotionDbRecordId",
                table: "QuestionAnswer",
                column: "NotionDbRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Progress");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "QuestionAnswer");

            migrationBuilder.DropTable(
                name: "LearningPlanQuestion");

            migrationBuilder.DropTable(
                name: "NotionDbRecords");

            migrationBuilder.DropTable(
                name: "LearningPlans");
        }
    }
}
