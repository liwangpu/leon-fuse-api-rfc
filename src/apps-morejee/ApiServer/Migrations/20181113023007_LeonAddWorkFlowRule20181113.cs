using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddWorkFlowRule20181113 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkFlowRuleDetails",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    KeyWord = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    WorkFlowId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowRuleDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowRules",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ActiveFlag = table.Column<int>(nullable: false),
                    CategoryId = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Creator = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    IsInner = table.Column<bool>(nullable: false),
                    Keyword = table.Column<string>(nullable: true),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    Modifier = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    ResourceType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowRules", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkFlowRuleDetails");

            migrationBuilder.DropTable(
                name: "WorkFlowRules");
        }
    }
}
