using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddSomeFieldsToWorkFlow20181108 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplyOrgans",
                table: "WorkFlows",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlowGrade",
                table: "WorkFlowItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyOrgans",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "FlowGrade",
                table: "WorkFlowItems");
        }
    }
}
