using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddGradeArchiveModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivedLearningPathGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LearningPathId = table.Column<int>(type: "int", nullable: false),
                    LearningPathName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AcademicYear = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EducationLevel = table.Column<int>(type: "int", nullable: false),
                    ClassLevel = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    SemesterStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SemesterEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TotalStudentsInPath = table.Column<int>(type: "int", nullable: false),
                    AverageClassGrade = table.Column<double>(type: "double", nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedLearningPathGrades", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArchivedStudentGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArchivedLearningPathGradeId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SemesterOverallGrade = table.Column<double>(type: "double", nullable: false),
                    StudentRank = table.Column<int>(type: "int", nullable: false),
                    PromotionGrade = table.Column<double>(type: "double", nullable: false),
                    PresentDays = table.Column<int>(type: "int", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    AttendanceRate = table.Column<double>(type: "double", nullable: false),
                    IsPromoted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PromotionStatus = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedStudentGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedStudentGrades_ArchivedLearningPathGrades_ArchivedLea~",
                        column: x => x.ArchivedLearningPathGradeId,
                        principalTable: "ArchivedLearningPathGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArchivedCourseGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArchivedStudentGradeId = table.Column<int>(type: "int", nullable: false),
                    Course = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalGrade = table.Column<double>(type: "double", nullable: false),
                    FinalGradeCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HomeworkWeightPercentage = table.Column<double>(type: "double", nullable: false),
                    QuizWeightPercentage = table.Column<double>(type: "double", nullable: false),
                    FinalExamWeightPercentage = table.Column<double>(type: "double", nullable: false),
                    HomeworkAverage = table.Column<double>(type: "double", nullable: false),
                    QuizAverage = table.Column<double>(type: "double", nullable: false),
                    ExamAverage = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedCourseGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedCourseGrades_ArchivedStudentGrades_ArchivedStudentGr~",
                        column: x => x.ArchivedStudentGradeId,
                        principalTable: "ArchivedStudentGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArchivedTestGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArchivedCourseGradeId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "double", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GradeType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedTestGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedTestGrades_ArchivedCourseGrades_ArchivedCourseGradeId",
                        column: x => x.ArchivedCourseGradeId,
                        principalTable: "ArchivedCourseGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedCourseGrades_ArchivedStudentGradeId",
                table: "ArchivedCourseGrades",
                column: "ArchivedStudentGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedStudentGrades_ArchivedLearningPathGradeId",
                table: "ArchivedStudentGrades",
                column: "ArchivedLearningPathGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedTestGrades_ArchivedCourseGradeId",
                table: "ArchivedTestGrades",
                column: "ArchivedCourseGradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedTestGrades");

            migrationBuilder.DropTable(
                name: "ArchivedCourseGrades");

            migrationBuilder.DropTable(
                name: "ArchivedStudentGrades");

            migrationBuilder.DropTable(
                name: "ArchivedLearningPathGrades");
        }
    }
}
