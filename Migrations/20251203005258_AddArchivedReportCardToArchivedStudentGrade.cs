using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddArchivedReportCardToArchivedStudentGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArchivedReportCardId",
                table: "ArchivedStudentGrades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedStudentGrades_ArchivedReportCardId",
                table: "ArchivedStudentGrades",
                column: "ArchivedReportCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedStudentGrades_StudentReportCards_ArchivedReportCardId",
                table: "ArchivedStudentGrades",
                column: "ArchivedReportCardId",
                principalTable: "StudentReportCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedStudentGrades_StudentReportCards_ArchivedReportCardId",
                table: "ArchivedStudentGrades");

            migrationBuilder.DropIndex(
                name: "IX_ArchivedStudentGrades_ArchivedReportCardId",
                table: "ArchivedStudentGrades");

            migrationBuilder.DropColumn(
                name: "ArchivedReportCardId",
                table: "ArchivedStudentGrades");
        }
    }
}
