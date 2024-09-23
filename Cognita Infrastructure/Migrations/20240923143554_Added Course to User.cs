using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cognita_API.Migrations
{
    /// <inheritdoc />
    public partial class AddedCoursetoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_CourseId",
                table: "User",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Course_CourseId",
                table: "User",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Course_CourseId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CourseId",
                table: "User");
        }
    }
}
