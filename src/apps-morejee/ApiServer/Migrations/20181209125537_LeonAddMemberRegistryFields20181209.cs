using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddMemberRegistryFields20181209 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "MemberRegistries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessCard",
                table: "MemberRegistries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "MemberRegistries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "MemberRegistries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "MemberRegistries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "MemberRegistries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "MemberRegistries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "MemberRegistries");

            migrationBuilder.DropColumn(
                name: "BusinessCard",
                table: "MemberRegistries");

            migrationBuilder.DropColumn(
                name: "City",
                table: "MemberRegistries");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "MemberRegistries");

            migrationBuilder.DropColumn(
                name: "Mail",
                table: "MemberRegistries");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "MemberRegistries");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "MemberRegistries");
        }
    }
}
