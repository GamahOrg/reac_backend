using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class addingNewTableForAmountsLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LitigationRequestAmountLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigAmount__3214EC0729841352", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LitigationRequestAmountLog_LitigationRequest_LitigationRequestId",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LitigationRequestAmountLog_SysLitigationRequestAmount_AmountId",
                        column: x => x.AmountId,
                        principalTable: "SysLitigationRequestAmount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_RequestId",
                table: "Invoices",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestAmountLog_AmountId",
                table: "LitigationRequestAmountLog",
                column: "AmountId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestAmountLog_LitigationRequestId",
                table: "LitigationRequestAmountLog",
                column: "LitigationRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequest_SysInvoice",
                table: "Invoices",
                column: "RequestId",
                principalTable: "LitigationRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequest_SysInvoice",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "LitigationRequestAmountLog");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_RequestId",
                table: "Invoices");
        }
    }
}
