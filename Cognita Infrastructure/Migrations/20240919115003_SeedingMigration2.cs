using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cognita_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedingMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityType",
                columns: table => new
                {
                    ActivityTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityType", x => x.ActivityTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_ActivityTypeId",
                table: "Activity",
                column: "ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_ActivityType_ActivityTypeId",
                table: "Activity",
                column: "ActivityTypeId",
                principalTable: "ActivityType",
                principalColumn: "ActivityTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_ActivityType_ActivityTypeId",
                table: "Activity");

            migrationBuilder.DropTable(
                name: "ActivityType");

            migrationBuilder.DropIndex(
                name: "IX_Activity_ActivityTypeId",
                table: "Activity");
        }
    }
}
