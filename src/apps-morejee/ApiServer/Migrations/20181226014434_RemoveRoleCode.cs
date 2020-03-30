using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class RemoveRoleCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "TypeCode",
                table: "OrganizationTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeCode",
                table: "OrganizationTypes",
                nullable: true);
        }
    }
}
