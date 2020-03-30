using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddNav20180906 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Navigations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    LValue = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NodeType = table.Column<string>(nullable: true),
                    ObjId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: true),
                    Permission = table.Column<string>(nullable: true),
                    RValue = table.Column<int>(nullable: false),
                    Role = table.Column<string>(nullable: true),
                    RootOrganizationId = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Navigations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Navigations");
        }
    }
}
