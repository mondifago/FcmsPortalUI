using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class CorrectAcademicPeriods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicPeriods_School_SchoolId",
                table: "AcademicPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningPaths_AcademicPeriods_AcademicPeriodId",
                table: "LearningPaths");

            migrationBuilder.DropIndex(
                name: "IX_LearningPaths_AcademicPeriodId",
                table: "LearningPaths");

            migrationBuilder.DropIndex(
                name: "IX_AcademicPeriods_SchoolId",
                table: "AcademicPeriods");

            migrationBuilder.DropColumn(
                name: "AcademicPeriodId",
                table: "LearningPaths");

            migrationBuilder.AddColumn<int>(
                name: "CurrentAcademicPeriodId",
                table: "School",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentAcademicPeriodId1",
                table: "School",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_School_CurrentAcademicPeriodId1",
                table: "School",
                column: "CurrentAcademicPeriodId1");

            migrationBuilder.AddForeignKey(
                name: "FK_School_AcademicPeriods_CurrentAcademicPeriodId1",
                table: "School",
                column: "CurrentAcademicPeriodId1",
                principalTable: "AcademicPeriods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_School_AcademicPeriods_CurrentAcademicPeriodId1",
                table: "School");

            migrationBuilder.DropIndex(
                name: "IX_School_CurrentAcademicPeriodId1",
                table: "School");

            migrationBuilder.DropColumn(
                name: "CurrentAcademicPeriodId",
                table: "School");

            migrationBuilder.DropColumn(
                name: "CurrentAcademicPeriodId1",
                table: "School");

            migrationBuilder.AddColumn<int>(
                name: "AcademicPeriodId",
                table: "LearningPaths",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_AcademicPeriodId",
                table: "LearningPaths",
                column: "AcademicPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPeriods_SchoolId",
                table: "AcademicPeriods",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicPeriods_School_SchoolId",
                table: "AcademicPeriods",
                column: "SchoolId",
                principalTable: "School",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningPaths_AcademicPeriods_AcademicPeriodId",
                table: "LearningPaths",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id");
        }
    }
}
