using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftScale.Modules.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                schema: "catalog",
                table: "Products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                schema: "catalog",
                table: "Products");
        }
    }
}
