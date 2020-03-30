using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class AddOrderDetailPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderDetailPackages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ProductPackageId = table.Column<string>(nullable: true),
                    Num = table.Column<int>(nullable: false),
                    OrderDetailId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetailPackages_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailPackages_OrderDetailId",
                table: "OrderDetailPackages",
                column: "OrderDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetailPackages");
        }
    }
}
