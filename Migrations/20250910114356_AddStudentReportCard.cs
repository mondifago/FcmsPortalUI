using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentReportCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentReportCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    LearningPathId = table.Column<int>(type: "int", nullable: false),
                    TeacherRemarks = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrincipalRemarks = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SemesterOverallGrade = table.Column<double>(type: "double", nullable: false),
                    PromotionGrade = table.Column<double>(type: "double", nullable: false),
                    StudentRank = table.Column<int>(type: "int", nullable: false),
                    PresentDays = table.Column<int>(type: "int", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    AttendanceRate = table.Column<double>(type: "double", nullable: false),
                    IsPromoted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PromotionStatus = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateGenerated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateFinalized = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsFinalized = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GeneratedByTeacherId = table.Column<int>(type: "int", nullable: true),
                    FinalizedByPrincipalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentReportCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentReportCards_LearningPaths_LearningPathId",
                        column: x => x.LearningPathId,
                        principalTable: "LearningPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentReportCards_Staff_FinalizedByPrincipalId",
                        column: x => x.FinalizedByPrincipalId,
                        principalTable: "Staff",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentReportCards_Staff_GeneratedByTeacherId",
                        column: x => x.GeneratedByTeacherId,
                        principalTable: "Staff",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentReportCards_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReportCards_FinalizedByPrincipalId",
                table: "StudentReportCards",
                column: "FinalizedByPrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReportCards_GeneratedByTeacherId",
                table: "StudentReportCards",
                column: "GeneratedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReportCards_LearningPathId",
                table: "StudentReportCards",
                column: "LearningPathId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReportCards_StudentId",
                table: "StudentReportCards",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentReportCards");
        }
    }
}
