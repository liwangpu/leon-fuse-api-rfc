using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class orderAddActiveFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveFlag",
                table: "Orders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveFlag",
                table: "Orders");
        }
    }
}
