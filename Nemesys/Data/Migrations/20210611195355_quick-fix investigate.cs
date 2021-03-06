using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nemesys.Data.Migrations
{
    public partial class quickfixinvestigate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EditUserRole");

            migrationBuilder.DropTable(
                name: "NewInvestigationViewModel");

            migrationBuilder.DropTable(
                name: "ReportViewModel");

            migrationBuilder.DropTable(
                name: "UserViewModel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EditUserRole",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditUserRole", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "UserViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HazardDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Upvotes = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportViewModel_UserViewModel_UserId",
                        column: x => x.UserId,
                        principalTable: "UserViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewInvestigationViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateOfAction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestigateViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestigateViewModel_ReportViewModel_ReportId",
                        column: x => x.ReportId,
                        principalTable: "ReportViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestigateViewModel_ReportId",
                table: "NewInvestigationViewModel",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportViewModel_UserId",
                table: "ReportViewModel",
                column: "UserId");
        }
    }
}
