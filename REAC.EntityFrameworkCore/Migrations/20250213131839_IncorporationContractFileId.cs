using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class IncorporationContractFileId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IncorporationContractFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_IncorporationContractFileId",
                table: "LitigationRequestContract",
                column: "IncorporationContractFileId");

            migrationBuilder.AddForeignKey(
                name: "FKIncorporationContractFileId807894",
                table: "LitigationRequestContract",
                column: "IncorporationContractFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FKIncorporationContractFileId807894",
                table: "LitigationRequestContract");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequestContract_IncorporationContractFileId",
                table: "LitigationRequestContract");

            migrationBuilder.DropColumn(
                name: "IncorporationContractFileId",
                table: "LitigationRequestContract");
        }
    }
}
