using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPoint.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsSubscribedToEventsProperty_To_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribedToEvents",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribedToEvents",
                table: "Users");
        }
    }
}
