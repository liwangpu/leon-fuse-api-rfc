using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApiServer.Migrations
{
    public partial class LeonChangeStaticmeshMaterialPro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "Materials",
                table: "StaticMeshs",
                newName: "DyMaterials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DyMaterials",
                table: "StaticMeshs",
                newName: "Materials");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "Layouts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Textures_FileAssetId",
                table: "Textures",
                column: "FileAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticMeshs_FileAssetId",
                table: "StaticMeshs",
                column: "FileAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_FileAssetId",
                table: "Materials",
                column: "FileAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_FileAssetId",
                table: "Maps",
                column: "FileAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maps_Files_FileAssetId",
                table: "Maps",
                column: "FileAssetId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Files_FileAssetId",
                table: "Materials",
                column: "FileAssetId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaticMeshs_Files_FileAssetId",
                table: "StaticMeshs",
                column: "FileAssetId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Textures_Files_FileAssetId",
                table: "Textures",
                column: "FileAssetId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
