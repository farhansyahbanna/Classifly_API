using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classifly_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDamageReportToBorrowRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_damage_reports_items_ItemId",
                table: "damage_reports");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "damage_reports",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "damage_reports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AdminMessage",
                table: "damage_reports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "BorrowRequestId",
                table: "damage_reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DamageReportId",
                table: "damage_reports",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "damage_reports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "damage_reports",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_BorrowRequestId",
                table: "damage_reports",
                column: "BorrowRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_DamageReportId",
                table: "damage_reports",
                column: "DamageReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_damage_reports_borrow_requests_BorrowRequestId",
                table: "damage_reports",
                column: "BorrowRequestId",
                principalTable: "borrow_requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_damage_reports_damage_reports_DamageReportId",
                table: "damage_reports",
                column: "DamageReportId",
                principalTable: "damage_reports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_damage_reports_items_ItemId",
                table: "damage_reports",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_damage_reports_borrow_requests_BorrowRequestId",
                table: "damage_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_damage_reports_damage_reports_DamageReportId",
                table: "damage_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_damage_reports_items_ItemId",
                table: "damage_reports");

            migrationBuilder.DropIndex(
                name: "IX_damage_reports_BorrowRequestId",
                table: "damage_reports");

            migrationBuilder.DropIndex(
                name: "IX_damage_reports_DamageReportId",
                table: "damage_reports");

            migrationBuilder.DropColumn(
                name: "BorrowRequestId",
                table: "damage_reports");

            migrationBuilder.DropColumn(
                name: "DamageReportId",
                table: "damage_reports");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "damage_reports");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "damage_reports");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "damage_reports",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "damage_reports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdminMessage",
                table: "damage_reports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_damage_reports_items_ItemId",
                table: "damage_reports",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
