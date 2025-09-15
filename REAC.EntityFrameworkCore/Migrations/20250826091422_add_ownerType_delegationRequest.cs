using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class add_ownerType_delegationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LitigationRequestOwnerTypeId",
                table: "DelegationRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DelegationRequest_LitigationRequestOwnerTypeId",
                table: "DelegationRequest",
                column: "LitigationRequestOwnerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DelegationRequest_SysLitigationRequestOwnerType_LitigationRequestOwnerTypeId",
                table: "DelegationRequest",
                column: "LitigationRequestOwnerTypeId",
                principalTable: "SysLitigationRequestOwnerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelegationRequest_SysLitigationRequestOwnerType_LitigationRequestOwnerTypeId",
                table: "DelegationRequest");

            migrationBuilder.DropIndex(
                name: "IX_DelegationRequest_LitigationRequestOwnerTypeId",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "LitigationRequestOwnerTypeId",
                table: "DelegationRequest");
        }
    }
}
