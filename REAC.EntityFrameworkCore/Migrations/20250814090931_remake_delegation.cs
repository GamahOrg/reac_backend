using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class remake_delegation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DelegatorId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LawLicenseFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LawyerLicenseExpirationDate",
                table: "Delegator",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LawyerLicenseNumber",
                table: "Delegator",
                type: "nvarchar(max)",
                nullable: true);

            

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_DelegatorId",
                table: "LitigationRequest",
                column: "DelegatorId");

            migrationBuilder.AddForeignKey(
                name: "FKDelegator_LitigationRequests",
                table: "LitigationRequest",
                column: "DelegatorId",
                principalTable: "Delegator",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FKDelegator_LitigationRequests",
                table: "LitigationRequest");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequest_DelegatorId",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "DelegatorId",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "LawLicenseFileId",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "LawyerLicenseExpirationDate",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "LawyerLicenseNumber",
                table: "Delegator");

            
        }
    }
}
