using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPoint.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Добавить поле Project (хранится как строка, дефолт — Pazl)
            migrationBuilder.AddColumn<string>(
                name: "Project",
                table: "Users",
                type: "longtext",
                nullable: false,
                defaultValue: "Pazl")
                .Annotation("MySql:CharSet", "utf8mb4");

            // Direction ранее хранил значения Pazl/Code (старый enum).
            // Сбрасываем невалидные значения в Backend.
            migrationBuilder.Sql(
                "UPDATE Users SET Direction = 'Backend' " +
                "WHERE Direction NOT IN ('Backend', 'Frontend', 'Design')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Project",
                table: "Users");
        }
    }
}
