using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestNote_Citizen_CitizenId",
                table: "LitigationRequestNote");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequestNote_CitizenId",
                table: "LitigationRequestNote");

            migrationBuilder.DropColumn(
                name: "CitizenId",
                table: "LitigationRequestNote");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestNote_CreatedBy",
                table: "LitigationRequestNote",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestNote_Citizen_CreatedBy",
                table: "LitigationRequestNote",
                column: "CreatedBy",
                principalTable: "Citizen",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestNote_Citizen_CreatedBy",
                table: "LitigationRequestNote");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequestNote_CreatedBy",
                table: "LitigationRequestNote");

            migrationBuilder.AddColumn<Guid>(
                name: "CitizenId",
                table: "LitigationRequestNote",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestNote_CitizenId",
                table: "LitigationRequestNote",
                column: "CitizenId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestNote_Citizen_CitizenId",
                table: "LitigationRequestNote",
                column: "CitizenId",
                principalTable: "Citizen",
                principalColumn: "Id");
        }
    }
}
