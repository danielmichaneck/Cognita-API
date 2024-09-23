using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cognita_API.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoletoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_ActivityType_ActivityTypeId",
                table: "Activity");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ActivityTypeId",
                table: "Activity",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_ActivityType_ActivityTypeId",
                table: "Activity",
                column: "ActivityTypeId",
                principalTable: "ActivityType",
                principalColumn: "ActivityTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_ActivityType_ActivityTypeId",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityTypeId",
                table: "Activity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_ActivityType_ActivityTypeId",
                table: "Activity",
                column: "ActivityTypeId",
                principalTable: "ActivityType",
                principalColumn: "ActivityTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
