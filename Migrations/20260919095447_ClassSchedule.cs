using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class ClassSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_Staff_TeacherId",
                table: "ClassSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntries_ClassSessions_ClassSessionId",
                table: "ScheduleEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntries_LearningPaths_LearningPathId",
                table: "ScheduleEntries");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntries_ClassSessionId",
                table: "ScheduleEntries");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntries_LearningPathId_DateTime",
                table: "ScheduleEntries");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_ClassLevel_Semester_Course_SessionNumber",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "ClassSessionId",
                table: "ScheduleEntries");

            migrationBuilder.DropColumn(
                name: "LearningPathId",
                table: "ScheduleEntries");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "LearningPaths");

            migrationBuilder.DropColumn(
                name: "TemplateKey",
                table: "LearningPaths");

            migrationBuilder.CreateTable(
                name: "ClassSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClassLevel = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Venue = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClassSessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_ClassSessions_ClassSessionId",
                        column: x => x.ClassSessionId,
                        principalTable: "ClassSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_ClassLevel_Semester_AcademicYearStart",
                table: "LearningPaths",
                columns: new[] { "ClassLevel", "Semester", "AcademicYearStart" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_ClassLevel_Semester_DateTime",
                table: "ClassSchedules",
                columns: new[] { "ClassLevel", "Semester", "DateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_ClassSessionId",
                table: "ClassSchedules",
                column: "ClassSessionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_Staff_TeacherId",
                table: "ClassSessions",
                column: "TeacherId",
                principalTable: "Staff",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_Staff_TeacherId",
                table: "ClassSessions");

            migrationBuilder.DropTable(
                name: "ClassSchedules");

            migrationBuilder.DropIndex(
                name: "IX_LearningPaths_ClassLevel_Semester_AcademicYearStart",
                table: "LearningPaths");

            migrationBuilder.AddColumn<int>(
                name: "ClassSessionId",
                table: "ScheduleEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LearningPathId",
                table: "ScheduleEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "LearningPaths",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TemplateKey",
                table: "LearningPaths",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntries_ClassSessionId",
                table: "ScheduleEntries",
                column: "ClassSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntries_LearningPathId_DateTime",
                table: "ScheduleEntries",
                columns: new[] { "LearningPathId", "DateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_ClassLevel_Semester_Course_SessionNumber",
                table: "ClassSessions",
                columns: new[] { "ClassLevel", "Semester", "Course", "SessionNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_Staff_TeacherId",
                table: "ClassSessions",
                column: "TeacherId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntries_ClassSessions_ClassSessionId",
                table: "ScheduleEntries",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntries_LearningPaths_LearningPathId",
                table: "ScheduleEntries",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id");
        }
    }
}
