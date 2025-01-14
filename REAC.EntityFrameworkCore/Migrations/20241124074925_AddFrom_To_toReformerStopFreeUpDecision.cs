using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddFrom_To_toReformerStopFreeUpDecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestCorrespondence_SysLitigationRequestCorrespondenceStatus_LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionFrom",
                table: "ReformerStopFreeUpDecision",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionTo",
                table: "ReformerStopFreeUpDecision",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestCorrespondence_SysLitigationRequestCorrespondenceStatus_LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                column: "LitigationRequestCorrespondenceStatusId",
                principalTable: "SysLitigationRequestCorrespondenceStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestCorrespondence_SysLitigationRequestCorrespondenceStatus_LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropColumn(
                name: "DecisionFrom",
                table: "ReformerStopFreeUpDecision");

            migrationBuilder.DropColumn(
                name: "DecisionTo",
                table: "ReformerStopFreeUpDecision");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestCorrespondence_SysLitigationRequestCorrespondenceStatus_LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                column: "LitigationRequestCorrespondenceStatusId",
                principalTable: "SysLitigationRequestCorrespondenceStatus",
                principalColumn: "Id");
        }
    }
}
