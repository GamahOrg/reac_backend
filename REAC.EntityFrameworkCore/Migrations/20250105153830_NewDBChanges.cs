using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class NewDBChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ArbitrationFees",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsArbitrationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCollectionOfArbitrationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCollectionOfLitigationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCollectionOfReconciliationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLitigationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReconciliationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ReconciliationFees",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeesPaid",
                table: "CitizenRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SigntureFileId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_SigntureFileId",
                table: "Citizen",
                column: "SigntureFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Citizen_SysFileManager_SigntureFileId",
                table: "Citizen",
                column: "SigntureFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citizen_SysFileManager_SigntureFileId",
                table: "Citizen");

            migrationBuilder.DropIndex(
                name: "IX_Citizen_SigntureFileId",
                table: "Citizen");

            migrationBuilder.DropColumn(
                name: "ArbitrationFees",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsArbitrationFeesHaveBeenCollected",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsCollectionOfArbitrationFeesRequestIsBeingSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsCollectionOfLitigationFeesRequestIsBeingSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsCollectionOfReconciliationFeesRequestIsBeingSent",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsLitigationFeesHaveBeenCollected",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsReconciliationFeesHaveBeenCollected",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "ReconciliationFees",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IsFeesPaid",
                table: "CitizenRequest");

            migrationBuilder.DropColumn(
                name: "SigntureFileId",
                table: "Citizen");
        }
    }
}
