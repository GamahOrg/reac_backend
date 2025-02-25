using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLitigationRequestFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fees",
                table: "LitigationRequestFees");

            migrationBuilder.DropColumn(
                name: "ToAmount",
                table: "LitigationRequestFees");

            migrationBuilder.RenameColumn(
                name: "FromAmount",
                table: "LitigationRequestFees",
                newName: "ArbitrationFees");

            migrationBuilder.AddColumn<Guid>(
                name: "AmountId",
                table: "LitigationRequestFees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestFees_AmountId",
                table: "LitigationRequestFees",
                column: "AmountId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestFees_SysLitigationRequestAmount_AmountId",
                table: "LitigationRequestFees",
                column: "AmountId",
                principalTable: "SysLitigationRequestAmount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestFees_SysLitigationRequestAmount_AmountId",
                table: "LitigationRequestFees");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequestFees_AmountId",
                table: "LitigationRequestFees");

            migrationBuilder.DropColumn(
                name: "AmountId",
                table: "LitigationRequestFees");

            migrationBuilder.RenameColumn(
                name: "ArbitrationFees",
                table: "LitigationRequestFees",
                newName: "FromAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "Fees",
                table: "LitigationRequestFees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ToAmount",
                table: "LitigationRequestFees",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
