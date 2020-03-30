using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class AddColorToSomeET : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Textures",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "StaticMeshs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Solutions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ProductSpec",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ProductGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Packages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Materials",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Maps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Layouts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Textures");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "StaticMeshs");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ProductSpec");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ProductGroups");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Layouts");
        }
    }
}
