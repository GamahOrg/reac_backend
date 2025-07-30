using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Add_LitigationRequestReconciliationReportDecisionPeriod_litigationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLitigationRequestReconciliationReportDecisionPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLitigationRequestReconciliationReportDecisionPeriodSent",
                table: "LitigationRequest");
        }
    }
}
