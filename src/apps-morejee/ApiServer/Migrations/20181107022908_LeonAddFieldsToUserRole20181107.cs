using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddFieldsToUserRole20181107 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveFlag",
                table: "UserRoles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "UserRoles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInner",
                table: "UserRoles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "UserRoles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Modifier",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResourceType",
                table: "UserRoles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveFlag",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Creator",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsInner",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Modifier",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "UserRoles");
        }
    }
}
