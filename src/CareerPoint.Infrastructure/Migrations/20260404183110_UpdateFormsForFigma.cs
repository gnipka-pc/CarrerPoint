using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPoint.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFormsForFigma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Forms: IsActive -> IsOpen ─────────────────────────────────────
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Forms",
                newName: "IsOpen");

            // ── Forms: добавить DeadlineAt ────────────────────────────────────
            migrationBuilder.AddColumn<DateTime>(
                name: "DeadlineAt",
                table: "Forms",
                type: "datetime(6)",
                nullable: true);

            // ── QuestionOptions: добавить Value ───────────────────────────────
            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "QuestionOptions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeadlineAt",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "QuestionOptions");

            migrationBuilder.RenameColumn(
                name: "IsOpen",
                table: "Forms",
                newName: "IsActive");
        }
    }
}
