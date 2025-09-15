using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Delegation_Requests_New : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FKDelegation266019",
                table: "DelegationRequest");

            migrationBuilder.DropForeignKey(
                name: "FKDelegation632464",
                table: "DelegationRequest");

            migrationBuilder.DropIndex(
                name: "IX_DelegationRequest_DelegationFileId",
                table: "DelegationRequest");

            migrationBuilder.DropIndex(
                name: "IX_DelegationRequest_IdentityTypeId",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "DelegationFileId",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "DelegationNumber",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "IdentityNumber",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "SourceCitizenId",
                table: "DelegationRequest");

            migrationBuilder.RenameColumn(
                name: "IdentityTypeId",
                table: "DelegationRequest",
                newName: "DelegationParty");

            

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "Facility",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorCitizenId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestedOn",
                table: "DelegationRequest",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<Guid>(
                name: "DelegatorId",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DelegationRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LitigationRequestId",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: true);

            

            migrationBuilder.CreateIndex(
                name: "IX_DelegationRequest_DelegatorId",
                table: "DelegationRequest",
                column: "DelegatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationRequest_LitigationRequestId",
                table: "DelegationRequest",
                column: "LitigationRequestId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_DelegationRequest_Delegator_DelegatorId",
                table: "DelegationRequest",
                column: "DelegatorId",
                principalTable: "Delegator",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DelegationRequest_LitigationRequest_LitigationRequestId",
                table: "DelegationRequest",
                column: "LitigationRequestId",
                principalTable: "LitigationRequest",
                principalColumn: "Id");

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
            migrationBuilder.DropForeignKey(
                name: "FK_DelegationRequest_Delegator_DelegatorId",
                table: "DelegationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegationRequest_LitigationRequest_LitigationRequestId",
                table: "DelegationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegationRequest_SysFileManager_SysFileManagerId",
                table: "DelegationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegationRequest_SysIdentityType_SysIdentityTypeId",
                table: "DelegationRequest");

            migrationBuilder.DropIndex(
                name: "IX_DelegationRequest_DelegatorId",
                table: "DelegationRequest");

            migrationBuilder.DropIndex(
                name: "IX_DelegationRequest_LitigationRequestId",
                table: "DelegationRequest");

            migrationBuilder.DropIndex(
                name: "IX_DelegationRequest_SysFileManagerId",
                table: "DelegationRequest");

            migrationBuilder.DropIndex(
                name: "IX_DelegationRequest_SysIdentityTypeId",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "DelegatorId",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "LitigationRequestId",
                table: "DelegationRequest");

            migrationBuilder.DropColumn(
                name: "SysIdentityTypeId",
                table: "DelegationRequest");

            migrationBuilder.RenameColumn(
                name: "SysFileManagerId",
                table: "DelegationRequest",
                newName: "CitizenId");

            migrationBuilder.RenameColumn(
                name: "DelegationParty",
                table: "DelegationRequest",
                newName: "IdentityTypeId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "Facility",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorCitizenId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestedOn",
                table: "DelegationRequest",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "DelegationRequest",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DelegationFileId",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DelegationNumber",
                table: "DelegationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "DelegationRequest",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityNumber",
                table: "DelegationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "DelegationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceCitizenId",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DelegationRequest_DelegationFileId",
                table: "DelegationRequest",
                column: "DelegationFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationRequest_IdentityTypeId",
                table: "DelegationRequest",
                column: "IdentityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FKDelegation266019",
                table: "DelegationRequest",
                column: "DelegationFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FKDelegation632464",
                table: "DelegationRequest",
                column: "IdentityTypeId",
                principalTable: "SysIdentityType",
                principalColumn: "Id");
        }
    }
}
