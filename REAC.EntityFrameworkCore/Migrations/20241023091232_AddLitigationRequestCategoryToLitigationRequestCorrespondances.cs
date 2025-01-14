using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddLitigationRequestCategoryToLitigationRequestCorrespondances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.AddColumn<int>(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondence_CreatedBy",
                table: "LitigationRequestCorrespondence",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondence_LitigationRequestCategoryId",
                table: "LitigationRequestCorrespondence",
                column: "LitigationRequestCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestCorrespondence_Citizen_CreatedBy",
                table: "LitigationRequestCorrespondence",
                column: "CreatedBy",
                principalTable: "Citizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestCorrespondence_SysLitigationRequestCategory_LitigationRequestCategoryId",
                table: "LitigationRequestCorrespondence",
                column: "LitigationRequestCategoryId",
                principalTable: "SysLitigationRequestCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestCorrespondence_Citizen_CreatedBy",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestCorrespondence_SysLitigationRequestCategory_LitigationRequestCategoryId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequestCorrespondence_CreatedBy",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequestCorrespondence_LitigationRequestCategoryId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropColumn(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "LitigationRequestCorrespondence",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "LitigationRequestCorrespondence",
                type: "datetime2",
                nullable: true);
        }
    }
}
