using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LitigationRequestInvoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    litigationRequestId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    invDateJ = table.Column<DateTime>(type: "datetime2", nullable: true),
                    sadadNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    sadadStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    invoiceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    invDateH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    invTime = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    isDeleted = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    deletedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    project = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    client = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    createdBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    invDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    invoicedNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    provinceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    neighborhoodName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    totalQuantity = table.Column<int>(type: "int", nullable: true),
                    totalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    totalNonTaxable = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    totalTaxable = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    totalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    totalTotalAfterTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    statusId = table.Column<int>(type: "int", nullable: true),
                    updatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createdOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitigationRequestInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LitigationRequestInvoice_LitigationRequest_litigationRequestId",
                        column: x => x.litigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestInvoice_litigationRequestId",
                table: "LitigationRequestInvoice",
                column: "litigationRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LitigationRequestInvoice");
        }
    }
}
