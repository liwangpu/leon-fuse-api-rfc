using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonChangeAccountRole20181121 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountRole_Accounts_AccountId",
                table: "AccountRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountRole",
                table: "AccountRole");

            migrationBuilder.RenameTable(
                name: "AccountRole",
                newName: "AccountRoles");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRoles",
                newName: "IX_AccountRoles_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountRoles",
                table: "AccountRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRoles_Accounts_AccountId",
                table: "AccountRoles",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountRoles_Accounts_AccountId",
                table: "AccountRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountRoles",
                table: "AccountRoles");

            migrationBuilder.RenameTable(
                name: "AccountRoles",
                newName: "AccountRole");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRoles_AccountId",
                table: "AccountRole",
                newName: "IX_AccountRole_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountRole",
                table: "AccountRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRole_Accounts_AccountId",
                table: "AccountRole",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
