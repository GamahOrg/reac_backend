using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class LitigationRequest_Add_New_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLitigationRequestArbitrationFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLitigationRequestReconciliationFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLitigationRequestArbitrationFeesNotPaidFirstReminderSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsLitigationRequestReconciliationFeesNotPaidFirstReminderSent",
                table: "LitigationRequest");
        }
    }
}
