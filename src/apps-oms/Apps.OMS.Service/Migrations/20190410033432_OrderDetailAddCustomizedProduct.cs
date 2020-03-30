using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class OrderDetailAddCustomizedProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolutionId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "OrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "OrderDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderDetailCustomizedProduct",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    OrderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailCustomizedProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetailCustomizedProduct_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailCustomizedProduct_OrderId",
                table: "OrderDetailCustomizedProduct",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetailCustomizedProduct");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SolutionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "OrderDetails");
        }
    }
}
