using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCalendarModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntries_CalendarModel_CalendarModelId",
                table: "ScheduleEntries");

            migrationBuilder.DropTable(
                name: "CalendarModel");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntries_CalendarModelId",
                table: "ScheduleEntries");

            migrationBuilder.DropColumn(
                name: "CalendarModelId",
                table: "ScheduleEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalendarModelId",
                table: "ScheduleEntries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CalendarModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchoolId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarModel_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntries_CalendarModelId",
                table: "ScheduleEntries",
                column: "CalendarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarModel_SchoolId",
                table: "CalendarModel",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntries_CalendarModel_CalendarModelId",
                table: "ScheduleEntries",
                column: "CalendarModelId",
                principalTable: "CalendarModel",
                principalColumn: "Id");
        }
    }
}
