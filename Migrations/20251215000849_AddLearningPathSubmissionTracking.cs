using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningPathSubmissionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateSubmitted",
                table: "LearningPaths",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmittedById",
                table: "LearningPaths",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByName",
                table: "LearningPaths",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSubmitted",
                table: "LearningPaths");

            migrationBuilder.DropColumn(
                name: "SubmittedById",
                table: "LearningPaths");

            migrationBuilder.DropColumn(
                name: "SubmittedByName",
                table: "LearningPaths");
        }
    }
}
