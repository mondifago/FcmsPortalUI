using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningPathAndSchoolPaymentArchives2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivedLearningPathPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LearningPathId = table.Column<int>(type: "int", nullable: false),
                    LearningPathName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EducationLevel = table.Column<int>(type: "int", nullable: false),
                    ClassLevel = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    AcademicYear = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalStudentsInPath = table.Column<int>(type: "int", nullable: false),
                    FeePerStudent = table.Column<double>(type: "double", nullable: false),
                    LearningPathExpectedRevenue = table.Column<double>(type: "double", nullable: false),
                    TotalPaid = table.Column<double>(type: "double", nullable: false),
                    Outstanding = table.Column<double>(type: "double", nullable: false),
                    LearningPathPaymentCompletionRate = table.Column<double>(type: "double", nullable: false),
                    AverageStudentPaymentCompletionRateInPath = table.Column<double>(type: "double", nullable: false),
                    LearningPathTimelyCompletionRate = table.Column<double>(type: "double", nullable: false),
                    AverageStudentTimelyCompletionRateInPath = table.Column<double>(type: "double", nullable: false),
                    SemesterStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SemesterEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedLearningPathPayments", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArchivedSchoolPaymentSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AcademicYear = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    TotalLearningPaths = table.Column<int>(type: "int", nullable: false),
                    TotalStudents = table.Column<int>(type: "int", nullable: false),
                    FullyPaidStudents = table.Column<int>(type: "int", nullable: false),
                    StudentsWithBalance = table.Column<int>(type: "int", nullable: false),
                    TotalExpectedRevenue = table.Column<double>(type: "double", nullable: false),
                    TotalAmountReceived = table.Column<double>(type: "double", nullable: false),
                    TotalOutstandingBalance = table.Column<double>(type: "double", nullable: false),
                    SchoolWidePaymentCompletionRate = table.Column<double>(type: "double", nullable: false),
                    SchoolWideTimelyCompletionRate = table.Column<double>(type: "double", nullable: false),
                    AverageStudentPaymentCompletionRateInSchool = table.Column<double>(type: "double", nullable: false),
                    AverageStudentTimelyCompletionRateInSchool = table.Column<double>(type: "double", nullable: false),
                    SemesterStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SemesterEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedSchoolPaymentSummaries", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedLearningPathPayments");

            migrationBuilder.DropTable(
                name: "ArchivedSchoolPaymentSummaries");
        }
    }
}
