using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemesys.Data.Migrations
{
    public partial class updatingReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReporterId",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReporterId",
                table: "Reports");
        }
    }
}
