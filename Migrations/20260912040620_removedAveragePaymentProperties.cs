using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class removedAveragePaymentProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageStudentPaymentCompletionRateInSchool",
                table: "ArchivedSchoolPaymentSummaries");

            migrationBuilder.DropColumn(
                name: "AverageStudentTimelyCompletionRateInSchool",
                table: "ArchivedSchoolPaymentSummaries");

            migrationBuilder.DropColumn(
                name: "AverageStudentPaymentCompletionRateInPath",
                table: "ArchivedLearningPathPayments");

            migrationBuilder.DropColumn(
                name: "AverageStudentTimelyCompletionRateInPath",
                table: "ArchivedLearningPathPayments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageStudentPaymentCompletionRateInSchool",
                table: "ArchivedSchoolPaymentSummaries",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AverageStudentTimelyCompletionRateInSchool",
                table: "ArchivedSchoolPaymentSummaries",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AverageStudentPaymentCompletionRateInPath",
                table: "ArchivedLearningPathPayments",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AverageStudentTimelyCompletionRateInPath",
                table: "ArchivedLearningPathPayments",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
