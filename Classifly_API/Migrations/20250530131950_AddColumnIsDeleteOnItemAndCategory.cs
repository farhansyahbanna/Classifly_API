using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classifly_API.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnIsDeleteOnItemAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "items");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "categories");
        }
    }
}
