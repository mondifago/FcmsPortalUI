using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class ArchivedStudentPaymentProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BroughtForward",
                table: "ArchivedStudentPayments",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CarriedForward",
                table: "ArchivedStudentPayments",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "ArchivedStudentPayments",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "StudentAddress",
                table: "ArchivedStudentPayments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "TermFee",
                table: "ArchivedStudentPayments",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalPayable",
                table: "ArchivedStudentPayments",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BroughtForward",
                table: "ArchivedStudentPayments");

            migrationBuilder.DropColumn(
                name: "CarriedForward",
                table: "ArchivedStudentPayments");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ArchivedStudentPayments");

            migrationBuilder.DropColumn(
                name: "StudentAddress",
                table: "ArchivedStudentPayments");

            migrationBuilder.DropColumn(
                name: "TermFee",
                table: "ArchivedStudentPayments");

            migrationBuilder.DropColumn(
                name: "TotalPayable",
                table: "ArchivedStudentPayments");
        }
    }
}
