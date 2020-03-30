using Microsoft.EntityFrameworkCore.Migrations;

namespace Apps.OMS.Service.Migrations
{
    public partial class changeRuleDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeyWord",
                table: "WorkFlowRuleDetails",
                newName: "WorkFlowRuleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkFlowRuleId",
                table: "WorkFlowRuleDetails",
                newName: "KeyWord");
        }
    }
}
