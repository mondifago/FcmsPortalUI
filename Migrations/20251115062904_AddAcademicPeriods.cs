using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicPeriods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicPeriodId",
                table: "LearningPaths",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcademicPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearStart = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    SemesterStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SemesterEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExamsStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsArchived = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicPeriods_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_AcademicPeriodId",
                table: "LearningPaths",
                column: "AcademicPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPeriods_SchoolId",
                table: "AcademicPeriods",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningPaths_AcademicPeriods_AcademicPeriodId",
                table: "LearningPaths",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningPaths_AcademicPeriods_AcademicPeriodId",
                table: "LearningPaths");

            migrationBuilder.DropTable(
                name: "AcademicPeriods");

            migrationBuilder.DropIndex(
                name: "IX_LearningPaths_AcademicPeriodId",
                table: "LearningPaths");

            migrationBuilder.DropColumn(
                name: "AcademicPeriodId",
                table: "LearningPaths");
        }
    }
}
