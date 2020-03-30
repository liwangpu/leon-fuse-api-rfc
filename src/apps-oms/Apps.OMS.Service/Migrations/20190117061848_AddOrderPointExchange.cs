using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class AddOrderPointExchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderPointExchanges",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Rate = table.Column<decimal>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPointExchanges", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPointExchanges");
        }
    }
}
