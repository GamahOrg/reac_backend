using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Delegator_LitigationRequest_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FKDelegator_LitigationRequests",
                table: "LitigationRequest");

            migrationBuilder.AddForeignKey(
                name: "FKDelegator_LitigationRequests",
                table: "LitigationRequest",
                column: "DelegatorId",
                principalTable: "Delegator",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FKDelegator_LitigationRequests",
                table: "LitigationRequest");

            migrationBuilder.AddForeignKey(
                name: "FKDelegator_LitigationRequests",
                table: "LitigationRequest",
                column: "DelegatorId",
                principalTable: "Delegator",
                principalColumn: "Id");
        }
    }
}
