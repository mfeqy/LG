using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Youxel.Check.LicensesGenerator.Migrations
{
    public partial class AddLicenseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Module = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfAdminUsers = table.Column<int>(type: "int", nullable: true),
                    NumberOfUnitUsers = table.Column<int>(type: "int", nullable: true),
                    NumberOfLocationUsers = table.Column<int>(type: "int", nullable: true),
                    NumberOfEndUsers = table.Column<int>(type: "int", nullable: true),
                    NumberOfUsers = table.Column<int>(type: "int", nullable: false),
                    NumberOfLocations = table.Column<int>(type: "int", nullable: false),
                    NumberOfAssets = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatabaseServer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MachineKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Licenses");
        }
    }
}
