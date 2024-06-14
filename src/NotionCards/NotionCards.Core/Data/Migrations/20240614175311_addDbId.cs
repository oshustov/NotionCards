using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotionCards.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDbId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Sets",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "NotionDbId",
                table: "NotionDbRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "NotionDbId",
                table: "NotionDbRecords");
        }
    }
}
