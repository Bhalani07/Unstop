using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnstopAPI.Migrations
{
    /// <inheritdoc />
    public partial class MinMaxSalaryToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Salary",
                table: "Jobs",
                newName: "MinSalary");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxSalary",
                table: "Jobs",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Occupancy",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxSalary",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Occupancy",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "MinSalary",
                table: "Jobs",
                newName: "Salary");
        }
    }
}
