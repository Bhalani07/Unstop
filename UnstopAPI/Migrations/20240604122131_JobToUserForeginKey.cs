using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnstopAPI.Migrations
{
    /// <inheritdoc />
    public partial class JobToUserForeginKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_UserId",
                table: "Jobs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_UserId",
                table: "Jobs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_UserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_UserId",
                table: "Jobs");
        }
    }
}
