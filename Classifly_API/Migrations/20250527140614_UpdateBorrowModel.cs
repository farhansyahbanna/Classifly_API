using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classifly_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBorrowModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_borrow_requests_items_ItemId",
                table: "borrow_requests");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "borrow_requests");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "borrow_requests",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "borrow_requests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "borrow_requests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "borrow_items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_borrow_requests_items_ItemId",
                table: "borrow_requests",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_borrow_requests_items_ItemId",
                table: "borrow_requests");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "borrow_requests");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "borrow_requests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "borrow_items");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "borrow_requests",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "borrow_requests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_borrow_requests_items_ItemId",
                table: "borrow_requests",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
