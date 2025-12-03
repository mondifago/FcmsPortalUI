using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAgeAndAllSemesterGradesToArchiveGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FirstSemesterGrade",
                table: "ArchivedStudentGrades",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SecondSemesterGrade",
                table: "ArchivedStudentGrades",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "StudentAge",
                table: "ArchivedStudentGrades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ThirdSemesterGrade",
                table: "ArchivedStudentGrades",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstSemesterGrade",
                table: "ArchivedStudentGrades");

            migrationBuilder.DropColumn(
                name: "SecondSemesterGrade",
                table: "ArchivedStudentGrades");

            migrationBuilder.DropColumn(
                name: "StudentAge",
                table: "ArchivedStudentGrades");

            migrationBuilder.DropColumn(
                name: "ThirdSemesterGrade",
                table: "ArchivedStudentGrades");
        }
    }
}
