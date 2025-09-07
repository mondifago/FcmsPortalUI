using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcmsPortalUI.Migrations
{
    /// <inheritdoc />
    public partial class AddTestGradeDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGrade_CourseGradingConfiguration_GradingConfigurationId",
                table: "CourseGrade");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGrade_LearningPaths_LearningPathId",
                table: "CourseGrade");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGrade_Students_StudentId",
                table: "CourseGrade");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeworkSubmission_TestGrade_HomeworkGradeId",
                table: "HomeworkSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGrade_CourseGrade_CourseGradeId",
                table: "TestGrade");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGrade_Staff_TeacherId",
                table: "TestGrade");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestGrade",
                table: "TestGrade");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseGrade",
                table: "CourseGrade");

            migrationBuilder.RenameTable(
                name: "TestGrade",
                newName: "TestGrades");

            migrationBuilder.RenameTable(
                name: "CourseGrade",
                newName: "CourseGrades");

            migrationBuilder.RenameIndex(
                name: "IX_TestGrade_TeacherId",
                table: "TestGrades",
                newName: "IX_TestGrades_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_TestGrade_CourseGradeId",
                table: "TestGrades",
                newName: "IX_TestGrades_CourseGradeId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGrade_StudentId",
                table: "CourseGrades",
                newName: "IX_CourseGrades_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGrade_LearningPathId",
                table: "CourseGrades",
                newName: "IX_CourseGrades_LearningPathId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGrade_GradingConfigurationId",
                table: "CourseGrades",
                newName: "IX_CourseGrades_GradingConfigurationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestGrades",
                table: "TestGrades",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseGrades",
                table: "CourseGrades",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGrades_CourseGradingConfiguration_GradingConfiguration~",
                table: "CourseGrades",
                column: "GradingConfigurationId",
                principalTable: "CourseGradingConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGrades_LearningPaths_LearningPathId",
                table: "CourseGrades",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGrades_Students_StudentId",
                table: "CourseGrades",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HomeworkSubmission_TestGrades_HomeworkGradeId",
                table: "HomeworkSubmission",
                column: "HomeworkGradeId",
                principalTable: "TestGrades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestGrades_CourseGrades_CourseGradeId",
                table: "TestGrades",
                column: "CourseGradeId",
                principalTable: "CourseGrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestGrades_Staff_TeacherId",
                table: "TestGrades",
                column: "TeacherId",
                principalTable: "Staff",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGrades_CourseGradingConfiguration_GradingConfiguration~",
                table: "CourseGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGrades_LearningPaths_LearningPathId",
                table: "CourseGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGrades_Students_StudentId",
                table: "CourseGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeworkSubmission_TestGrades_HomeworkGradeId",
                table: "HomeworkSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGrades_CourseGrades_CourseGradeId",
                table: "TestGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGrades_Staff_TeacherId",
                table: "TestGrades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestGrades",
                table: "TestGrades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseGrades",
                table: "CourseGrades");

            migrationBuilder.RenameTable(
                name: "TestGrades",
                newName: "TestGrade");

            migrationBuilder.RenameTable(
                name: "CourseGrades",
                newName: "CourseGrade");

            migrationBuilder.RenameIndex(
                name: "IX_TestGrades_TeacherId",
                table: "TestGrade",
                newName: "IX_TestGrade_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_TestGrades_CourseGradeId",
                table: "TestGrade",
                newName: "IX_TestGrade_CourseGradeId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGrades_StudentId",
                table: "CourseGrade",
                newName: "IX_CourseGrade_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGrades_LearningPathId",
                table: "CourseGrade",
                newName: "IX_CourseGrade_LearningPathId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGrades_GradingConfigurationId",
                table: "CourseGrade",
                newName: "IX_CourseGrade_GradingConfigurationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestGrade",
                table: "TestGrade",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseGrade",
                table: "CourseGrade",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGrade_CourseGradingConfiguration_GradingConfigurationId",
                table: "CourseGrade",
                column: "GradingConfigurationId",
                principalTable: "CourseGradingConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGrade_LearningPaths_LearningPathId",
                table: "CourseGrade",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGrade_Students_StudentId",
                table: "CourseGrade",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HomeworkSubmission_TestGrade_HomeworkGradeId",
                table: "HomeworkSubmission",
                column: "HomeworkGradeId",
                principalTable: "TestGrade",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestGrade_CourseGrade_CourseGradeId",
                table: "TestGrade",
                column: "CourseGradeId",
                principalTable: "CourseGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestGrade_Staff_TeacherId",
                table: "TestGrade",
                column: "TeacherId",
                principalTable: "Staff",
                principalColumn: "Id");
        }
    }
}
