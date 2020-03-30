using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class AddFieldsToCategoryAndProductSpec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slots",
                table: "ProductSpec",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomData",
                table: "AssetCategories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slots",
                table: "ProductSpec");

            migrationBuilder.DropColumn(
                name: "CustomData",
                table: "AssetCategories");
        }
    }
}
