using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class delegation_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DisputeAmount",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "EstablishmentLetterFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the establishment Letter file");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_EstablishmentLetterFileId",
                table: "Delegator",
                column: "EstablishmentLetterFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegator_SysFileManager_EstablishmentLetterFileId",
                table: "Delegator",
                column: "EstablishmentLetterFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delegator_SysFileManager_EstablishmentLetterFileId",
                table: "Delegator");

            migrationBuilder.DropIndex(
                name: "IX_Delegator_EstablishmentLetterFileId",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "DisputeAmount",
                table: "LitigationRequest");

            migrationBuilder.DropColumn(
                name: "EstablishmentLetterFileId",
                table: "Delegator");
        }
    }
}
