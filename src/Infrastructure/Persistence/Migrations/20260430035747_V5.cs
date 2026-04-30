using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Projects",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "portuguese")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedBy",
                table: "Tasks",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedBy_Created",
                table: "Tasks",
                columns: new[] { "CreatedBy", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedBy_GoalId",
                table: "Tasks",
                columns: new[] { "CreatedBy", "GoalId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedBy_ProjectId",
                table: "Tasks",
                columns: new[] { "CreatedBy", "ProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedBy",
                table: "Tags",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_CreatedBy",
                table: "Reminders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_CreatedBy_RemindAt",
                table: "Reminders",
                columns: new[] { "CreatedBy", "RemindAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedBy",
                table: "Projects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedBy_Created",
                table: "Projects",
                columns: new[] { "CreatedBy", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_CreatedBy",
                table: "Goals",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_CreatedBy_Created",
                table: "Goals",
                columns: new[] { "CreatedBy", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_CreatedBy_ProjectId",
                table: "Goals",
                columns: new[] { "CreatedBy", "ProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoalLogs_CreatedBy",
                table: "GoalLogs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GoalLogs_CreatedBy_Date",
                table: "GoalLogs",
                columns: new[] { "CreatedBy", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_GoalLogs_CreatedBy_GoalId",
                table: "GoalLogs",
                columns: new[] { "CreatedBy", "GoalId" });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CreatedBy",
                table: "Attachments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CreatedBy_Created",
                table: "Attachments",
                columns: new[] { "CreatedBy", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CreatedBy_TaskItemId",
                table: "Attachments",
                columns: new[] { "CreatedBy", "TaskItemId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedBy",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedBy_Created",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedBy_GoalId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedBy_ProjectId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CreatedBy",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_CreatedBy",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_CreatedBy_RemindAt",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedBy",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedBy_Created",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Goals_CreatedBy",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_CreatedBy_Created",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_CreatedBy_ProjectId",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_GoalLogs_CreatedBy",
                table: "GoalLogs");

            migrationBuilder.DropIndex(
                name: "IX_GoalLogs_CreatedBy_Date",
                table: "GoalLogs");

            migrationBuilder.DropIndex(
                name: "IX_GoalLogs_CreatedBy_GoalId",
                table: "GoalLogs");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_CreatedBy",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_CreatedBy_Created",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_CreatedBy_TaskItemId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Projects");
        }
    }
}
