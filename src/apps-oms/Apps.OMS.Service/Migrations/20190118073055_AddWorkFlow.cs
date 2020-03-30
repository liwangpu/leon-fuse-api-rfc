using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class AddWorkFlow : Migration
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
                    Name = table.Column<string>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsInner = table.Column<bool>(nullable: false),
                    Keyword = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlows",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    ApplyOrgans = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    Creator = table.Column<string>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    WorkFlowId = table.Column<string>(nullable: true),
                    SubWorkFlowId = table.Column<string>(nullable: true),
                    OperateRoles = table.Column<string>(nullable: true),
                    FlowGrade = table.Column<int>(nullable: false),
                    AutoWorkFlow = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowItems_WorkFlows_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "WorkFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowItems_WorkFlowId",
                table: "WorkFlowItems",
                column: "WorkFlowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkFlowItems");

            migrationBuilder.DropTable(
                name: "WorkFlowRuleDetails");

            migrationBuilder.DropTable(
                name: "WorkFlowRules");

            migrationBuilder.DropTable(
                name: "WorkFlows");
        }
    }
}
