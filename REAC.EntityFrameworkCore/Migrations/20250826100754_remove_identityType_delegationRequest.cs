using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class remove_identityType_delegationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelegationRequest_SysIdentityType_SysIdentityTypeId",
                table: "DelegationRequest");

            migrationBuilder.DropIndex(
                name: "IX_DelegationRequest_SysIdentityTypeId",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "SysIdentityTypeId",
                table: "DelegationRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SysIdentityTypeId",
                table: "DelegationRequest",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DelegationRequest_SysIdentityTypeId",
                table: "DelegationRequest",
                column: "SysIdentityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DelegationRequest_SysIdentityType_SysIdentityTypeId",
                table: "DelegationRequest",
                column: "SysIdentityTypeId",
                principalTable: "SysIdentityType",
                principalColumn: "Id");
        }
    }
}
