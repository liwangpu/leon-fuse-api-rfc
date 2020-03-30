using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonChangeUserNavDetailField20181115 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Field",
                table: "UserNavDetails");

            migrationBuilder.RenameColumn(
                name: "Permission",
                table: "UserNavDetails",
                newName: "ExcludePermission");

            migrationBuilder.RenameColumn(
                name: "PagedModel",
                table: "UserNavDetails",
                newName: "ExcludeFiled");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExcludePermission",
                table: "UserNavDetails",
                newName: "Permission");

            migrationBuilder.RenameColumn(
                name: "ExcludeFiled",
                table: "UserNavDetails",
                newName: "PagedModel");

            migrationBuilder.AddColumn<string>(
                name: "Field",
                table: "UserNavDetails",
                nullable: true);
        }
    }
}
