using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddOrderDetail0913 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderStateItem");

            migrationBuilder.DropColumn(
                name: "ChildOrders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Orders");

            migrationBuilder.CreateTable(
                name: "OrderDetailStates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ActiveFlag = table.Column<int>(nullable: false),
                    CategoryId = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Creator = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    Modifier = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Num = table.Column<int>(nullable: false),
                    OrderDetailStateId = table.Column<int>(nullable: false),
                    OrderId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    ProductSpecId = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    ResourceType = table.Column<int>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_OrderDetailStates_OrderDetailStateId",
                        column: x => x.OrderDetailStateId,
                        principalTable: "OrderDetailStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderDetailStateId",
                table: "OrderDetails",
                column: "OrderDetailStateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderDetailStates");

            migrationBuilder.AddColumn<string>(
                name: "ChildOrders",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderStateItem",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Detail = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NewState = table.Column<string>(nullable: true),
                    OldState = table.Column<string>(nullable: true),
                    OperateTime = table.Column<DateTime>(nullable: false),
                    OperatorAccount = table.Column<string>(nullable: true),
                    OrderId = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    SolutionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStateItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStateItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderStateItem_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStateItem_OrderId",
                table: "OrderStateItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStateItem_SolutionId",
                table: "OrderStateItem",
                column: "SolutionId");
        }
    }
}
