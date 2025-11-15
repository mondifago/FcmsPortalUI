using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class CorrectAcademicPeriods2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_School_AcademicPeriods_CurrentAcademicPeriodId1",
                table: "School");

            migrationBuilder.DropIndex(
                name: "IX_School_CurrentAcademicPeriodId1",
                table: "School");

            migrationBuilder.DropColumn(
                name: "CurrentAcademicPeriodId1",
                table: "School");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "AcademicPeriods");

            migrationBuilder.CreateIndex(
                name: "IX_School_CurrentAcademicPeriodId",
                table: "School",
                column: "CurrentAcademicPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_School_AcademicPeriods_CurrentAcademicPeriodId",
                table: "School",
                column: "CurrentAcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_School_AcademicPeriods_CurrentAcademicPeriodId",
                table: "School");

            migrationBuilder.DropIndex(
                name: "IX_School_CurrentAcademicPeriodId",
                table: "School");

            migrationBuilder.AddColumn<int>(
                name: "CurrentAcademicPeriodId1",
                table: "School",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "AcademicPeriods",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}
