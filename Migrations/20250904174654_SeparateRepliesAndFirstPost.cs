using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class SeparateRepliesAndFirstPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionThreads_DiscussionPosts_FirstPostId",
                table: "DiscussionThreads");

            migrationBuilder.DropTable(
                name: "DiscussionPosts");

            migrationBuilder.DropIndex(
                name: "IX_DiscussionThreads_FirstPostId",
                table: "DiscussionThreads");

            migrationBuilder.DropColumn(
                name: "FirstPostId",
                table: "DiscussionThreads");

            migrationBuilder.CreateTable(
                name: "FirstPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscussionThreadId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirstPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FirstPosts_DiscussionThreads_DiscussionThreadId",
                        column: x => x.DiscussionThreadId,
                        principalTable: "DiscussionThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FirstPosts_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Replies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscussionThreadId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Replies_DiscussionThreads_DiscussionThreadId",
                        column: x => x.DiscussionThreadId,
                        principalTable: "DiscussionThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Replies_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FirstPosts_DiscussionThreadId",
                table: "FirstPosts",
                column: "DiscussionThreadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FirstPosts_PersonId",
                table: "FirstPosts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_DiscussionThreadId",
                table: "Replies",
                column: "DiscussionThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_PersonId",
                table: "Replies",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FirstPosts");

            migrationBuilder.DropTable(
                name: "Replies");

            migrationBuilder.AddColumn<int>(
                name: "FirstPostId",
                table: "DiscussionThreads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DiscussionPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscussionThreadId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionPosts_DiscussionThreads_DiscussionThreadId",
                        column: x => x.DiscussionThreadId,
                        principalTable: "DiscussionThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscussionPosts_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionThreads_FirstPostId",
                table: "DiscussionThreads",
                column: "FirstPostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionPosts_DiscussionThreadId",
                table: "DiscussionPosts",
                column: "DiscussionThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionPosts_PersonId",
                table: "DiscussionPosts",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionThreads_DiscussionPosts_FirstPostId",
                table: "DiscussionThreads",
                column: "FirstPostId",
                principalTable: "DiscussionPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
