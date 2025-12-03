using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddGuardianInfoToArchivedStudentGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuardianEmail",
                table: "ArchivedStudentGrades",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GuardianName",
                table: "ArchivedStudentGrades",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GuardianPhoneNumber",
                table: "ArchivedStudentGrades",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StudentEmail",
                table: "ArchivedStudentGrades",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuardianEmail",
                table: "ArchivedStudentGrades");

            migrationBuilder.DropColumn(
                name: "GuardianName",
                table: "ArchivedStudentGrades");

            migrationBuilder.DropColumn(
                name: "GuardianPhoneNumber",
                table: "ArchivedStudentGrades");

            migrationBuilder.DropColumn(
                name: "StudentEmail",
                table: "ArchivedStudentGrades");
        }
    }
}
