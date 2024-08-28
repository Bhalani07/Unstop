using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnstopAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedJobTabele : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "JobType",
                table: "Jobs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobTiming",
                table: "Jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxExperience",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinExperience",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "Jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingDays",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "JobTiming",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "MaxExperience",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "MinExperience",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "Jobs");

            migrationBuilder.AlterColumn<string>(
                name: "JobType",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
