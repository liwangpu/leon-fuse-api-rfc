using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddQueryParamFields20181119 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExcludeQueryParams",
                table: "UserNavDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueryParams",
                table: "Navigations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludeQueryParams",
                table: "UserNavDetails");

            migrationBuilder.DropColumn(
                name: "QueryParams",
                table: "Navigations");
        }
    }
}
