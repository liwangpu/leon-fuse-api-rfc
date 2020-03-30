using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddMemberRegistry20181207 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFolders_Accounts_AccountId",
                table: "AssetFolders");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientAssets_Accounts_AccountId",
                table: "ClientAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Accounts_AccountId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Layouts_Accounts_AccountId",
                table: "Layouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Accounts_AccountId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Accounts_AccountId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Accounts_AccountId",
                table: "Solutions");

            migrationBuilder.DropTable(
                name: "AccountOpenId");

            migrationBuilder.DropTable(
                name: "OrganMember");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_AccountId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Products_AccountId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AccountId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Layouts_AccountId",
                table: "Layouts");

            migrationBuilder.DropIndex(
                name: "IX_Files_AccountId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_ClientAssets_AccountId",
                table: "ClientAssets");

            migrationBuilder.DropIndex(
                name: "IX_AssetFolders_AccountId",
                table: "AssetFolders");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Layouts");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "ClientAssets");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AssetFolders");

            migrationBuilder.CreateTable(
                name: "MemberTrees",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    LValue = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NodeType = table.Column<string>(nullable: true),
                    ObjId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: true),
                    RValue = table.Column<int>(nullable: false),
                    RootOrganizationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberTrees", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberTrees");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Solutions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Layouts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "ClientAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "AssetFolders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountOpenId",
                columns: table => new
                {
                    OpenId = table.Column<string>(nullable: false),
                    AccountId = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Platform = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountOpenId", x => x.OpenId);
                    table.ForeignKey(
                        name: "FK_AccountOpenId_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganMember",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccountId = table.Column<string>(nullable: true),
                    ActiveFlag = table.Column<int>(nullable: false),
                    CategoryId = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Creator = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    JoinDepartmentTime = table.Column<DateTime>(nullable: false),
                    JoinOrganTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    Modifier = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    ResourceType = table.Column<int>(nullable: false),
                    Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganMember_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganMember_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganMember_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_AccountId",
                table: "Solutions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_AccountId",
                table: "Products",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AccountId",
                table: "Orders",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_AccountId",
                table: "Layouts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_AccountId",
                table: "Files",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAssets_AccountId",
                table: "ClientAssets",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFolders_AccountId",
                table: "AssetFolders",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountOpenId_AccountId",
                table: "AccountOpenId",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganMember_AccountId",
                table: "OrganMember",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganMember_DepartmentId",
                table: "OrganMember",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganMember_OrganizationId",
                table: "OrganMember",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFolders_Accounts_AccountId",
                table: "AssetFolders",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAssets_Accounts_AccountId",
                table: "ClientAssets",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Accounts_AccountId",
                table: "Files",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Layouts_Accounts_AccountId",
                table: "Layouts",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Accounts_AccountId",
                table: "Orders",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Accounts_AccountId",
                table: "Products",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Accounts_AccountId",
                table: "Solutions",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
