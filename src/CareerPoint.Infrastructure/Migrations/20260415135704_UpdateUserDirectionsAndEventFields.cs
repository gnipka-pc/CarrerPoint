using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPoint.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDirectionsAndEventFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direction",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Directions",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ExternalUrl",
                table: "Events",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "Events",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EventFavorites",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "UTC_TIMESTAMP()",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURRENT_TIMESTAMP()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Directions",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExternalUrl",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "Direction",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EventFavorites",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP()",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "UTC_TIMESTAMP()");
        }
    }
}
