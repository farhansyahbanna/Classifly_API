using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classifly_API.Migrations
{
    /// <inheritdoc />
    public partial class FixDamageReportId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_damage_reports_damage_reports_DamageReportId",
                table: "damage_reports");

            migrationBuilder.DropIndex(
                name: "IX_damage_reports_DamageReportId",
                table: "damage_reports");

            migrationBuilder.DropColumn(
                name: "DamageReportId",
                table: "damage_reports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DamageReportId",
                table: "damage_reports",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_DamageReportId",
                table: "damage_reports",
                column: "DamageReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_damage_reports_damage_reports_DamageReportId",
                table: "damage_reports",
                column: "DamageReportId",
                principalTable: "damage_reports",
                principalColumn: "Id");
        }
    }
}
