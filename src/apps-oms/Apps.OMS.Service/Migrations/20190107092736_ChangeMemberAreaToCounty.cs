using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class ChangeMemberAreaToCounty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Area",
                table: "Members",
                newName: "County");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "MemberRegistries",
                newName: "County");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "County",
                table: "Members",
                newName: "Area");

            migrationBuilder.RenameColumn(
                name: "County",
                table: "MemberRegistries",
                newName: "Area");
        }
    }
}
