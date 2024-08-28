using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnstopAPI.Migrations
{
    /// <inheritdoc />
    public partial class CompanyTableWithLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Companies",
                newName: "LogoFileName");

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "Companies",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoContentType",
                table: "Companies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "LogoContentType",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "LogoFileName",
                table: "Companies",
                newName: "Description");
        }
    }
}
