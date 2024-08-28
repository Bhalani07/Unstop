using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnstopAPI.Migrations
{
    /// <inheritdoc />
    public partial class CandidateTableWithProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProfileCompleted",
                table: "Candidates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProfileCompleted",
                table: "Candidates");
        }
    }
}
