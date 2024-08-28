using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnstopAPI.Migrations
{
    /// <inheritdoc />
    public partial class CandidateConnectionWithApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_UserId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Applications",
                newName: "CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_UserId",
                table: "Applications",
                newName: "IX_Applications_CandidateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Candidates_CandidateId",
                table: "Applications",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "CandidateId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Candidates_CandidateId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "CandidateId",
                table: "Applications",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_CandidateId",
                table: "Applications",
                newName: "IX_Applications_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_UserId",
                table: "Applications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
