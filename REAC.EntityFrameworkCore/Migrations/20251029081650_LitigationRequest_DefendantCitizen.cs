using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class LitigationRequest_DefendantCitizen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<Guid>(
                name: "DefendantCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_DefendantCitizenId",
                table: "LitigationRequest",
                column: "DefendantCitizenId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequest_Citizen_DefendantCitizenId",
                table: "LitigationRequest",
                column: "DefendantCitizenId",
                principalTable: "Citizen",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequest_Citizen_DefendantCitizenId",
                table: "LitigationRequest");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequest_DefendantCitizenId",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "DefendantCitizenId",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "IndividualCapacity",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "Email2",
                table: "Citizen");

            migrationBuilder.DropColumn(
                name: "Mobile2",
                table: "Citizen");

            migrationBuilder.AlterColumn<decimal>(
                name: "DisputeAmount",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
