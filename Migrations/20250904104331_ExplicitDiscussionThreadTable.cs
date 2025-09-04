using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class ExplicitDiscussionThreadTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThread_ClassSessions_ClassSessionId",
                table: "DiscussionThread");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThread_DiscussionPost_FirstPostId",
                table: "DiscussionThread");

            migrationBuilder.DropIndex(
                name: "IX_DiscussionThread_FirstPostId",
                table: "DiscussionThread");

            migrationBuilder.AlterColumn<int>(
                name: "ClassSessionId",
                table: "DiscussionThread",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "DiscussionPost",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionThread_FirstPostId",
                table: "DiscussionThread",
                column: "FirstPostId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThread_ClassSessions_ClassSessionId",
                table: "DiscussionThread",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThread_DiscussionPost_FirstPostId",
                table: "DiscussionThread",
                column: "FirstPostId",
                principalTable: "DiscussionPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThread_ClassSessions_ClassSessionId",
                table: "DiscussionThread");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThread_DiscussionPost_FirstPostId",
                table: "DiscussionThread");

            migrationBuilder.DropIndex(
                name: "IX_DiscussionThread_FirstPostId",
                table: "DiscussionThread");

            migrationBuilder.AlterColumn<int>(
                name: "ClassSessionId",
                table: "DiscussionThread",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "DiscussionPost",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionThread_FirstPostId",
                table: "DiscussionThread",
                column: "FirstPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThread_ClassSessions_ClassSessionId",
                table: "DiscussionThread",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThread_DiscussionPost_FirstPostId",
                table: "DiscussionThread",
                column: "FirstPostId",
                principalTable: "DiscussionPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
