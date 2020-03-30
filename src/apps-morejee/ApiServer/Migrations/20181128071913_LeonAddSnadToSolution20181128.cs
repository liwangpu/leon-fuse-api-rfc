using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonAddSnadToSolution20181128 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Snapshot",
                table: "Solutions",
                newName: "IsSnapshot");

            migrationBuilder.AddColumn<string>(
                name: "SnapshotData",
                table: "Solutions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SnapshotData",
                table: "Solutions");

            migrationBuilder.RenameColumn(
                name: "IsSnapshot",
                table: "Solutions",
                newName: "Snapshot");
        }
    }
}
