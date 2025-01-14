using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddedCitizentoLititgationRequestModificationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LititgationRequestModificationRequest_CreatedBy",
                table: "LititgationRequestModificationRequest",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_LititgationRequestModificationRequest_Citizen_CreatedBy",
                table: "LititgationRequestModificationRequest",
                column: "CreatedBy",
                principalTable: "Citizen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LititgationRequestModificationRequest_Citizen_CreatedBy",
                table: "LititgationRequestModificationRequest");

            migrationBuilder.DropIndex(
                name: "IX_LititgationRequestModificationRequest_CreatedBy",
                table: "LititgationRequestModificationRequest");
        }
    }
}
