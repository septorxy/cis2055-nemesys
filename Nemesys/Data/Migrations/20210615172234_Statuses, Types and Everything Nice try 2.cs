using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemesys.Data.Migrations
{
    public partial class StatusesTypesandEverythingNicetry2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reports_StatusId",
                table: "Reports",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TypeId",
                table: "Reports",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Status_StatusId",
                table: "Reports",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Type_TypeId",
                table: "Reports",
                column: "TypeId",
                principalTable: "Type",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Status_StatusId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Type_TypeId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_StatusId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_TypeId",
                table: "Reports");
        }
    }
}
