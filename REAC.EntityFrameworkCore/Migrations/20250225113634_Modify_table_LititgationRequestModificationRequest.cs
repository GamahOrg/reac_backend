using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Modify_table_LititgationRequestModificationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LititgationRequestModificationRequest_LitigationRequestSession_LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "ActorType",
                table: "LititgationRequestModificationRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "LititgationRequestModificationRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LititgationRequestModificationRequest_LitigationRequestSession_LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest",
                column: "LitigationRequestSessionId",
                principalTable: "LitigationRequestSession",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LititgationRequestModificationRequest_LitigationRequestSession_LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest");

            migrationBuilder.DropColumn(
                name: "ActorType",
                table: "LititgationRequestModificationRequest");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "LititgationRequestModificationRequest");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LititgationRequestModificationRequest_LitigationRequestSession_LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest",
                column: "LitigationRequestSessionId",
                principalTable: "LitigationRequestSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
