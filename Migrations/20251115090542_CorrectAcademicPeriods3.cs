using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class CorrectAcademicPeriods3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AcademicPeriods");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "AcademicPeriods");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AcademicPeriods",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "AcademicPeriods",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
