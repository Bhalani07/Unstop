using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnstopAPI.Migrations
{
    /// <inheritdoc />
    public partial class ForeignkeyToElement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "Elements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Elements_TemplateId",
                table: "Elements",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_Templates_TemplateId",
                table: "Elements",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "TemplateId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_Templates_TemplateId",
                table: "Elements");

            migrationBuilder.DropIndex(
                name: "IX_Elements_TemplateId",
                table: "Elements");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Elements");
        }
    }
}
