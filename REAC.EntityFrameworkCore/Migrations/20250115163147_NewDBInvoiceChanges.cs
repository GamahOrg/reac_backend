using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class NewDBInvoiceChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FromAmount",
                table: "SysLitigationRequestAmount",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ToAmount",
                table: "SysLitigationRequestAmount",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LitigationRequestFees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorsCount = table.Column<int>(type: "int", nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExtraAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentageExtraAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Fees = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitigationRequestFees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysInvoiceStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysInvoiceStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysInvoiceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysInvoiceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceTypeId = table.Column<int>(type: "int", nullable: false),
                    InvoiceStatusId = table.Column<int>(type: "int", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_SysInvoiceStatus_InvoiceStatusId",
                        column: x => x.InvoiceStatusId,
                        principalTable: "SysInvoiceStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_SysInvoiceType_InvoiceTypeId",
                        column: x => x.InvoiceTypeId,
                        principalTable: "SysInvoiceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceStatusId",
                table: "Invoices",
                column: "InvoiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceTypeId",
                table: "Invoices",
                column: "InvoiceTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "LitigationRequestFees");

            migrationBuilder.DropTable(
                name: "SysInvoiceStatus");

            migrationBuilder.DropTable(
                name: "SysInvoiceType");

            migrationBuilder.DropColumn(
                name: "FromAmount",
                table: "SysLitigationRequestAmount");

            migrationBuilder.DropColumn(
                name: "ToAmount",
                table: "SysLitigationRequestAmount");
        }
    }
}
