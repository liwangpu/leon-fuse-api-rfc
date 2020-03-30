using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddLeonChangeOrder20181122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetailStates");

            migrationBuilder.DropColumn(
                name: "StateTime",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "WorkFlowId",
                table: "Orders",
                newName: "WorkFlowItemId");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Orders",
                newName: "SubOrderIds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkFlowItemId",
                table: "Orders",
                newName: "WorkFlowId");

            migrationBuilder.RenameColumn(
                name: "SubOrderIds",
                table: "Orders",
                newName: "State");

            migrationBuilder.AddColumn<DateTime>(
                name: "StateTime",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
        }
    }
}
