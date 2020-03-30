using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddOrderDetail09131 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderDetailStates_OrderDetailStateId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderDetailStateId",
                table: "OrderDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderDetailStateId",
                table: "OrderDetails",
                column: "OrderDetailStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderDetailStates_OrderDetailStateId",
                table: "OrderDetails",
                column: "OrderDetailStateId",
                principalTable: "OrderDetailStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
