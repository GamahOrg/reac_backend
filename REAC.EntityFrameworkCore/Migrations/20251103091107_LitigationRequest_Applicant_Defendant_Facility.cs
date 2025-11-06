using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class LitigationRequest_Applicant_Defendant_Facility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantFacilityId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefendantFacilityId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_ApplicantFacilityId",
                table: "LitigationRequest",
                column: "ApplicantFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_DefendantFacilityId",
                table: "LitigationRequest",
                column: "DefendantFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequest_Facility_ApplicantFacilityId",
                table: "LitigationRequest",
                column: "ApplicantFacilityId",
                principalTable: "Facility",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequest_Facility_DefendantFacilityId",
                table: "LitigationRequest",
                column: "DefendantFacilityId",
                principalTable: "Facility",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequest_Facility_ApplicantFacilityId",
                table: "LitigationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequest_Facility_DefendantFacilityId",
                table: "LitigationRequest");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequest_ApplicantFacilityId",
                table: "LitigationRequest");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequest_DefendantFacilityId",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "ApplicantFacilityId",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "DefendantFacilityId",
                table: "LitigationRequest");
        }
    }
}
