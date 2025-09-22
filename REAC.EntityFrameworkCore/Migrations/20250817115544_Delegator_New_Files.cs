using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Delegator_New_Files : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommercialRegisterationFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustodyFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DecisionFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EstablishmentFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RepresentationFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_CommercialRegisterationFileId",
                table: "Delegator",
                column: "CommercialRegisterationFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_CustodyFileId",
                table: "Delegator",
                column: "CustodyFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_DecisionFileId",
                table: "Delegator",
                column: "DecisionFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_EstablishmentFileId",
                table: "Delegator",
                column: "EstablishmentFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_RepresentationFileId",
                table: "Delegator",
                column: "RepresentationFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegator_SysFileManager_CommercialRegisterationFileId",
                table: "Delegator",
                column: "CommercialRegisterationFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegator_SysFileManager_CustodyFileId",
                table: "Delegator",
                column: "CustodyFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegator_SysFileManager_DecisionFileId",
                table: "Delegator",
                column: "DecisionFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegator_SysFileManager_EstablishmentFileId",
                table: "Delegator",
                column: "EstablishmentFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegator_SysFileManager_RepresentationFileId",
                table: "Delegator",
                column: "RepresentationFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delegator_SysFileManager_CommercialRegisterationFileId",
                table: "Delegator");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegator_SysFileManager_CustodyFileId",
                table: "Delegator");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegator_SysFileManager_DecisionFileId",
                table: "Delegator");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegator_SysFileManager_EstablishmentFileId",
                table: "Delegator");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegator_SysFileManager_RepresentationFileId",
                table: "Delegator");

            migrationBuilder.DropIndex(
                name: "IX_Delegator_CommercialRegisterationFileId",
                table: "Delegator");

            migrationBuilder.DropIndex(
                name: "IX_Delegator_CustodyFileId",
                table: "Delegator");

            migrationBuilder.DropIndex(
                name: "IX_Delegator_DecisionFileId",
                table: "Delegator");

            migrationBuilder.DropIndex(
                name: "IX_Delegator_EstablishmentFileId",
                table: "Delegator");

            migrationBuilder.DropIndex(
                name: "IX_Delegator_RepresentationFileId",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "CommercialRegisterationFileId",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "CustodyFileId",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "DecisionFileId",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "EstablishmentFileId",
                table: "Delegator");

            migrationBuilder.DropColumn(
                name: "RepresentationFileId",
                table: "Delegator");
        }
    }
}
