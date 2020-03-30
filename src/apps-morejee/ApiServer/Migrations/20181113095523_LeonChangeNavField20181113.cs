using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonChangeNavField20181113 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RootOrganizationId",
                table: "Navigations",
                newName: "Modifier");

            migrationBuilder.RenameColumn(
                name: "RValue",
                table: "Navigations",
                newName: "ResourceType");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Navigations",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ObjId",
                table: "Navigations",
                newName: "Creator");

            migrationBuilder.RenameColumn(
                name: "NodeType",
                table: "Navigations",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "LValue",
                table: "Navigations",
                newName: "ActiveFlag");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Navigations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "Navigations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Navigations");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "Navigations");

            migrationBuilder.RenameColumn(
                name: "ResourceType",
                table: "Navigations",
                newName: "RValue");

            migrationBuilder.RenameColumn(
                name: "Modifier",
                table: "Navigations",
                newName: "RootOrganizationId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Navigations",
                newName: "ParentId");

            migrationBuilder.RenameColumn(
                name: "Creator",
                table: "Navigations",
                newName: "ObjId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Navigations",
                newName: "NodeType");

            migrationBuilder.RenameColumn(
                name: "ActiveFlag",
                table: "Navigations",
                newName: "LValue");
        }
    }
}
