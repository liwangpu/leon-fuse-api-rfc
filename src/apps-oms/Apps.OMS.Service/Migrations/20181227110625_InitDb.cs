using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberHierarchyParams",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsInner = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberHierarchyParams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberHierarchySettings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: true),
                    MemberHierarchyParamId = table.Column<string>(nullable: true),
                    Rate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberHierarchySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberRegistries",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    ActiveFlag = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Mail = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    BusinessCard = table.Column<string>(nullable: true),
                    Inviter = table.Column<string>(nullable: true),
                    IsApprove = table.Column<bool>(nullable: false),
                    Approver = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRegistries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Creator = table.Column<string>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    Company = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    BusinessCard = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    Inviter = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberHierarchyParams");

            migrationBuilder.DropTable(
                name: "MemberHierarchySettings");

            migrationBuilder.DropTable(
                name: "MemberRegistries");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
