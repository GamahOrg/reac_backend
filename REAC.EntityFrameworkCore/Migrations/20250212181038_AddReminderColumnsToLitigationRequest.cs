using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderColumnsToLitigationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
            name: "IsReformerReminderForReconciliationSecondPeriodSent",
            table: "LitigationRequest",
            nullable: false,
            defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReformerFinalReminderForReconciliationSent",
                table: "LitigationRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReformerReminderForCancelReconciliationSent",
                table: "LitigationRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAssignNewReformerForCancelReconciliationSent",
                table: "LitigationRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArbitratorReminderForCancelArbitrationSent",
                table: "LitigationRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReminderForCancelLitigationForNotesSent",
                table: "LitigationRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReminderForCancelLitigationForAgreementSent",
                table: "LitigationRequest",
                nullable: false,
                defaultValue: false);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "IsReformerReminderForReconciliationSecondPeriodSent",
            table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsReformerFinalReminderForReconciliationSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsReformerReminderForCancelReconciliationSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsAssignNewReformerForCancelReconciliationSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsArbitratorReminderForCancelArbitrationSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsReminderForCancelLitigationForNotesSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsReminderForCancelLitigationForAgreementSent",
                table: "LitigationRequest");

        }
    }
}
