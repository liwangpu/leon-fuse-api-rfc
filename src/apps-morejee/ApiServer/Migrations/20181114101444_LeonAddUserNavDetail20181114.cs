using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddUserNavDetail20181114 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Field",
                table: "UserNavs");

            migrationBuilder.DropColumn(
                name: "PagedModel",
                table: "UserNavs");

            migrationBuilder.DropColumn(
                name: "Permission",
                table: "UserNavs");

            migrationBuilder.DropColumn(
                name: "RefNavigationId",
                table: "UserNavs");

            migrationBuilder.CreateTable(
                name: "UserNavDetails",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Field = table.Column<string>(nullable: true),
                    Grade = table.Column<int>(nullable: false),
                    PagedModel = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: true),
                    Permission = table.Column<string>(nullable: true),
                    RefNavigationId = table.Column<string>(nullable: true),
                    UserNavId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNavDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNavDetails_UserNavs_UserNavId",
                        column: x => x.UserNavId,
                        principalTable: "UserNavs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNavDetails_UserNavId",
                table: "UserNavDetails",
                column: "UserNavId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserNavDetails");

            migrationBuilder.AddColumn<string>(
                name: "Field",
                table: "UserNavs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PagedModel",
                table: "UserNavs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Permission",
                table: "UserNavs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefNavigationId",
                table: "UserNavs",
                nullable: true);
        }
    }
}
