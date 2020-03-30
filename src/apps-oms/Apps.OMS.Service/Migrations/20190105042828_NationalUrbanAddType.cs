using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class NationalUrbanAddType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalUrbanType",
                table: "NationalUrbans",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalUrbanType",
                table: "NationalUrbans");
        }
    }
}
