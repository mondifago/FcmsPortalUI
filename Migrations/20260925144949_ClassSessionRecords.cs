using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class ClassSessionRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThreads_ClassSessions_ClassSessionId",
                table: "DiscussionThreads");

            migrationBuilder.DropForeignKey(
                name: "FK_Homework_ClassSessions_ClassSessionId",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "RemarksSubmittedAt",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "RemarksSubmittedByName",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "TeacherRemarks",
                table: "ClassSessions");

            migrationBuilder.RenameColumn(
                name: "ClassSessionId",
                table: "Homework",
                newName: "ClassSessionRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_Homework_ClassSessionId",
                table: "Homework",
                newName: "IX_Homework_ClassSessionRecordId");

            migrationBuilder.RenameColumn(
                name: "ClassSessionId",
                table: "DiscussionThreads",
                newName: "ClassSessionRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionThreads_ClassSessionId",
                table: "DiscussionThreads",
                newName: "IX_DiscussionThreads_ClassSessionRecordId");

            migrationBuilder.CreateTable(
                name: "ClassSessionRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClassSessionId = table.Column<int>(type: "int", nullable: false),
                    AcademicPeriodId = table.Column<int>(type: "int", nullable: false),
                    TeacherRemarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksSubmittedByName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksSubmittedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ClosedByName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSessionRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSessionRecords_AcademicPeriods_AcademicPeriodId",
                        column: x => x.AcademicPeriodId,
                        principalTable: "AcademicPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassSessionRecords_ClassSessions_ClassSessionId",
                        column: x => x.ClassSessionId,
                        principalTable: "ClassSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessionRecords_AcademicPeriodId",
                table: "ClassSessionRecords",
                column: "AcademicPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessionRecords_ClassSessionId_AcademicPeriodId",
                table: "ClassSessionRecords",
                columns: new[] { "ClassSessionId", "AcademicPeriodId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThreads_ClassSessionRecords_ClassSessionRecordId",
                table: "DiscussionThreads",
                column: "ClassSessionRecordId",
                principalTable: "ClassSessionRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Homework_ClassSessionRecords_ClassSessionRecordId",
                table: "Homework",
                column: "ClassSessionRecordId",
                principalTable: "ClassSessionRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThreads_ClassSessionRecords_ClassSessionRecordId",
                table: "DiscussionThreads");

            migrationBuilder.DropForeignKey(
                name: "FK_Homework_ClassSessionRecords_ClassSessionRecordId",
                table: "Homework");

            migrationBuilder.DropTable(
                name: "ClassSessionRecords");

            migrationBuilder.RenameColumn(
                name: "ClassSessionRecordId",
                table: "Homework",
                newName: "ClassSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Homework_ClassSessionRecordId",
                table: "Homework",
                newName: "IX_Homework_ClassSessionId");

            migrationBuilder.RenameColumn(
                name: "ClassSessionRecordId",
                table: "DiscussionThreads",
                newName: "ClassSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionThreads_ClassSessionRecordId",
                table: "DiscussionThreads",
                newName: "IX_DiscussionThreads_ClassSessionId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "ClassSessions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemarksSubmittedAt",
                table: "ClassSessions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemarksSubmittedByName",
                table: "ClassSessions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TeacherRemarks",
                table: "ClassSessions",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThreads_ClassSessions_ClassSessionId",
                table: "DiscussionThreads",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Homework_ClassSessions_ClassSessionId",
                table: "Homework",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
