using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotionCards.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceKind = table.Column<int>(type: "int", nullable: false),
                    FrontText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoxCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    LeitnerBoxId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeitnerBoxCard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotionDbPulls",
                columns: table => new
                {
                    NotionDbId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastRecordDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotionDbPulls", x => x.NotionDbId);
                });

            migrationBuilder.CreateTable(
                name: "NotionDbs",
                columns: table => new
                {
                    NotionDbId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FieldMappings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotionDbName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PullingInterval = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotionDbs", x => x.NotionDbId);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSetState",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SetId = table.Column<int>(type: "int", nullable: false),
                    LastRequest = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSetState", x => new { x.UserId, x.SetId });
                });

            migrationBuilder.CreateTable(
                name: "Box",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    SetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeitnerBox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeitnerBox_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AddedTime",
                table: "Cards",
                column: "AddedTime",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_LeitnerBox_SetId",
                table: "Box",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_LeitnerBoxCard_LeitnerBoxId",
                table: "BoxCard",
                column: "LeitnerBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSetState_UserId_SetId",
                table: "UserSetState",
                columns: new[] { "UserId", "SetId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Box");

            migrationBuilder.DropTable(
                name: "BoxCard");

            migrationBuilder.DropTable(
                name: "NotionDbPulls");

            migrationBuilder.DropTable(
                name: "NotionDbs");

            migrationBuilder.DropTable(
                name: "UserSetState");

            migrationBuilder.DropTable(
                name: "Sets");
        }
    }
}
