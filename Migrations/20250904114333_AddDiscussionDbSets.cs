using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscussionDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionPost_DiscussionThread_DiscussionThreadId",
                table: "DiscussionPost");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionPost_Persons_PersonId",
                table: "DiscussionPost");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThread_ClassSessions_ClassSessionId",
                table: "DiscussionThread");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThread_DiscussionPost_FirstPostId",
                table: "DiscussionThread");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscussionThread",
                table: "DiscussionThread");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscussionPost",
                table: "DiscussionPost");

            migrationBuilder.RenameTable(
                name: "DiscussionThread",
                newName: "DiscussionThreads");

            migrationBuilder.RenameTable(
                name: "DiscussionPost",
                newName: "DiscussionPosts");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionThread_FirstPostId",
                table: "DiscussionThreads",
                newName: "IX_DiscussionThreads_FirstPostId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionThread_ClassSessionId",
                table: "DiscussionThreads",
                newName: "IX_DiscussionThreads_ClassSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionPost_PersonId",
                table: "DiscussionPosts",
                newName: "IX_DiscussionPosts_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionPost_DiscussionThreadId",
                table: "DiscussionPosts",
                newName: "IX_DiscussionPosts_DiscussionThreadId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscussionThreads",
                table: "DiscussionThreads",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscussionPosts",
                table: "DiscussionPosts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionPosts_DiscussionThreads_DiscussionThreadId",
                table: "DiscussionPosts",
                column: "DiscussionThreadId",
                principalTable: "DiscussionThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionPosts_Persons_PersonId",
                table: "DiscussionPosts",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThreads_ClassSessions_ClassSessionId",
                table: "DiscussionThreads",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThreads_DiscussionPosts_FirstPostId",
                table: "DiscussionThreads",
                column: "FirstPostId",
                principalTable: "DiscussionPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionPosts_DiscussionThreads_DiscussionThreadId",
                table: "DiscussionPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionPosts_Persons_PersonId",
                table: "DiscussionPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThreads_ClassSessions_ClassSessionId",
                table: "DiscussionThreads");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThreads_DiscussionPosts_FirstPostId",
                table: "DiscussionThreads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscussionThreads",
                table: "DiscussionThreads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscussionPosts",
                table: "DiscussionPosts");

            migrationBuilder.RenameTable(
                name: "DiscussionThreads",
                newName: "DiscussionThread");

            migrationBuilder.RenameTable(
                name: "DiscussionPosts",
                newName: "DiscussionPost");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionThreads_FirstPostId",
                table: "DiscussionThread",
                newName: "IX_DiscussionThread_FirstPostId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionThreads_ClassSessionId",
                table: "DiscussionThread",
                newName: "IX_DiscussionThread_ClassSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionPosts_PersonId",
                table: "DiscussionPost",
                newName: "IX_DiscussionPost_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionPosts_DiscussionThreadId",
                table: "DiscussionPost",
                newName: "IX_DiscussionPost_DiscussionThreadId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscussionThread",
                table: "DiscussionThread",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscussionPost",
                table: "DiscussionPost",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionPost_DiscussionThread_DiscussionThreadId",
                table: "DiscussionPost",
                column: "DiscussionThreadId",
                principalTable: "DiscussionThread",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionPost_Persons_PersonId",
                table: "DiscussionPost",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
