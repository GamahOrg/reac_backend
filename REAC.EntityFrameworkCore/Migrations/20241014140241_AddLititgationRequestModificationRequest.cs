using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddLititgationRequestModificationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LititgationRequestModificationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationRequest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LititgationRequestModificationRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LititgationRequestModificationRequest_LitigationRequestSession_LitigationRequestSessionId",
                        column: x => x.LitigationRequestSessionId,
                        principalTable: "LitigationRequestSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LititgationRequestModificationRequest_LitigationRequest_LitigationRequestId",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LititgationRequestModificationRequest_LitigationRequestId",
                table: "LititgationRequestModificationRequest",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LititgationRequestModificationRequest_LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest",
                column: "LitigationRequestSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LititgationRequestModificationRequest");
        }
    }
}
