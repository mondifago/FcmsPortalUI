using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleEntryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntry_CalendarModel_CalendarModelId",
                table: "ScheduleEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntry_ClassSessions_ClassSessionId",
                table: "ScheduleEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntry_LearningPaths_LearningPathId",
                table: "ScheduleEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleEntry",
                table: "ScheduleEntry");

            migrationBuilder.RenameTable(
                name: "ScheduleEntry",
                newName: "ScheduleEntries");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleEntry_LearningPathId",
                table: "ScheduleEntries",
                newName: "IX_ScheduleEntries_LearningPathId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleEntry_ClassSessionId",
                table: "ScheduleEntries",
                newName: "IX_ScheduleEntries_ClassSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleEntry_CalendarModelId",
                table: "ScheduleEntries",
                newName: "IX_ScheduleEntries_CalendarModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleEntries",
                table: "ScheduleEntries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntries_CalendarModel_CalendarModelId",
                table: "ScheduleEntries",
                column: "CalendarModelId",
                principalTable: "CalendarModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntries_ClassSessions_ClassSessionId",
                table: "ScheduleEntries",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntries_LearningPaths_LearningPathId",
                table: "ScheduleEntries",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntries_CalendarModel_CalendarModelId",
                table: "ScheduleEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntries_ClassSessions_ClassSessionId",
                table: "ScheduleEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntries_LearningPaths_LearningPathId",
                table: "ScheduleEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleEntries",
                table: "ScheduleEntries");

            migrationBuilder.RenameTable(
                name: "ScheduleEntries",
                newName: "ScheduleEntry");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleEntries_LearningPathId",
                table: "ScheduleEntry",
                newName: "IX_ScheduleEntry_LearningPathId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleEntries_ClassSessionId",
                table: "ScheduleEntry",
                newName: "IX_ScheduleEntry_ClassSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleEntries_CalendarModelId",
                table: "ScheduleEntry",
                newName: "IX_ScheduleEntry_CalendarModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleEntry",
                table: "ScheduleEntry",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntry_CalendarModel_CalendarModelId",
                table: "ScheduleEntry",
                column: "CalendarModelId",
                principalTable: "CalendarModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntry_ClassSessions_ClassSessionId",
                table: "ScheduleEntry",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntry_LearningPaths_LearningPathId",
                table: "ScheduleEntry",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id");
        }
    }
}
