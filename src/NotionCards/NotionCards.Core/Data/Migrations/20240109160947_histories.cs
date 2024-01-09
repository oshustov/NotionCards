using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotionCards.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class histories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotionDbId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastOperation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportHistories_NotionDbId",
                table: "ImportHistories",
                column: "NotionDbId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportHistories");
        }
    }
}
