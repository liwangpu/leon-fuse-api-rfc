using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddOrganTypeIdToOrgan20181213 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Organizations_ParentId",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_ParentId",
                table: "Organizations");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationTypeId",
                table: "Organizations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizationTypeId",
                table: "Organizations");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ParentId",
                table: "Organizations",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Organizations_ParentId",
                table: "Organizations",
                column: "ParentId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
