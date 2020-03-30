using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddOrderFlow20181122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderDetailStateId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<string>(
                name: "WorkFlowId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderFlowLogs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Approve = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Creator = table.Column<string>(nullable: true),
                    OrderId = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFlowLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderFlowLogs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderFlowLogs_OrderId",
                table: "OrderFlowLogs",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderFlowLogs");

            migrationBuilder.DropColumn(
                name: "WorkFlowId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrderDetailStateId",
                table: "OrderDetails",
                nullable: false,
                defaultValue: 0);
        }
    }
}
