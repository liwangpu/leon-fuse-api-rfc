using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDS.API.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "idsapp");

            migrationBuilder.CreateTable(
                name: "IdentityGrant",
                schema: "idsapp",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    SubjectId = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityGrant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                schema: "idsapp",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Mail = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    ExpireTime = table.Column<DateTime>(nullable: false),
                    ActivationTime = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Identity",
                schema: "idsapp",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ModifiedTime = table.Column<DateTime>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "idsapp",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "idsapp",
                table: "Organization",
                columns: new[] { "Id", "ActivationTime", "CreatedTime", "Creator", "Description", "ExpireTime", "Icon", "Location", "Mail", "ModifiedTime", "Modifier", "Name", "OwnerId", "ParentId", "Phone", "Type" },
                values: new object[] { "bamboo", new DateTime(2019, 3, 30, 22, 22, 42, 205, DateTimeKind.Local).AddTicks(6653), new DateTime(2020, 3, 30, 14, 22, 42, 206, DateTimeKind.Utc).AddTicks(3968), "admin", "", new DateTime(2120, 3, 30, 22, 22, 42, 205, DateTimeKind.Local).AddTicks(6529), null, null, "bamboo@bamboo", new DateTime(2020, 3, 30, 14, 22, 42, 206, DateTimeKind.Utc).AddTicks(3968), "admin", "竹烛", "admin", null, "157", null });

            migrationBuilder.InsertData(
                schema: "idsapp",
                table: "Identity",
                columns: new[] { "Id", "CreatedTime", "Creator", "Email", "ModifiedTime", "Modifier", "Name", "OrganizationId", "Password", "Phone", "Username" },
                values: new object[] { "admin", new DateTime(2020, 3, 30, 14, 22, 42, 200, DateTimeKind.Utc).AddTicks(5032), "admin", "bamboo@bamboo.com", new DateTime(2020, 3, 30, 14, 22, 42, 200, DateTimeKind.Utc).AddTicks(5032), "admin", "Admin", "bamboo", "e10adc3949ba59abbe56e057f20f883e", "157", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Identity_OrganizationId",
                schema: "idsapp",
                table: "Identity",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Identity",
                schema: "idsapp");

            migrationBuilder.DropTable(
                name: "IdentityGrant",
                schema: "idsapp");

            migrationBuilder.DropTable(
                name: "Organization",
                schema: "idsapp");
        }
    }
}
