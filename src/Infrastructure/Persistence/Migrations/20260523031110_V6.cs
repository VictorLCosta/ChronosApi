using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrashed",
                table: "Tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TrashedBy",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TrashedOnUtc",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Tasks",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Tags",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Reminders",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "identity",
                table: "RefreshTokens",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Projects",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrashed",
                table: "Goals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TrashedBy",
                table: "Goals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TrashedOnUtc",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Goals",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "GoalLogs",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "AuditTrails",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Attachments",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_IsTrashed",
                table: "Tasks",
                column: "IsTrashed",
                filter: "\"IsTrashed\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_IsTrashed",
                table: "Goals",
                column: "IsTrashed",
                filter: "\"IsTrashed\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DropIndex(
                name: "IX_Tasks_IsTrashed",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Goals_IsTrashed",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "IsTrashed",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TrashedBy",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TrashedOnUtc",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsTrashed",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "TrashedBy",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "TrashedOnUtc",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "GoalLogs");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Attachments");
        }
    }
}