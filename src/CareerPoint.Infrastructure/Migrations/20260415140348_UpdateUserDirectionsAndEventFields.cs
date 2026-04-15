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
            // Добавляем новое поле Directions только если его нет
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'CareerPoint'
                    AND TABLE_NAME = 'Users'
                    AND COLUMN_NAME = 'Directions')
                THEN
                    ALTER TABLE `Users` ADD `Directions` longtext CHARACTER SET utf8mb4 NOT NULL;
                END IF;
            ");

            // Удаляем старое поле Direction только если оно существует
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'CareerPoint'
                    AND TABLE_NAME = 'Users'
                    AND COLUMN_NAME = 'Direction')
                THEN
                    ALTER TABLE `Users` DROP COLUMN `Direction`;
                END IF;
            ");

            // Добавляем ExternalUrl только если его нет
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'CareerPoint'
                    AND TABLE_NAME = 'Events'
                    AND COLUMN_NAME = 'ExternalUrl')
                THEN
                    ALTER TABLE `Events` ADD `ExternalUrl` longtext CHARACTER SET utf8mb4 NULL;
                END IF;
            ");

            // Добавляем Salary только если его нет
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'CareerPoint'
                    AND TABLE_NAME = 'Events'
                    AND COLUMN_NAME = 'Salary')
                THEN
                    ALTER TABLE `Events` ADD `Salary` decimal(65,30) NULL;
                END IF;
            ");

            // Обновляем CreatedAt в EventFavorites только если таблица существует
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'CareerPoint'
                    AND TABLE_NAME = 'EventFavorites')
                THEN
                    ALTER TABLE `EventFavorites` MODIFY COLUMN `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6);
                END IF;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventFavorites");

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
        }
    }
}
