using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddLitigationRequestCorrespondenceReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LitigationRequestCorrespondenceReply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestCorrespondenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReplyMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitigationRequestCorrespondenceReply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LitigationRequestCorrespondenceReply_Citizen_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Citizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LitigationRequestCorrespondenceReply_LitigationRequestCorrespondence_LitigationRequestCorrespondenceId",
                        column: x => x.LitigationRequestCorrespondenceId,
                        principalTable: "LitigationRequestCorrespondence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestCorrespondenceReplyFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestCorrespondenceReplyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitigationRequestCorrespondenceReplyFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LitigationRequestCorrespondenceReplyFile_LitigationRequestCorrespondenceReply_LitigationRequestCorrespondenceReplyId",
                        column: x => x.LitigationRequestCorrespondenceReplyId,
                        principalTable: "LitigationRequestCorrespondenceReply",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LitigationRequestCorrespondenceReplyFile_SysFileManager_FileManagerId",
                        column: x => x.FileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondenceReply_CreatedBy",
                table: "LitigationRequestCorrespondenceReply",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondenceReply_LitigationRequestCorrespondenceId",
                table: "LitigationRequestCorrespondenceReply",
                column: "LitigationRequestCorrespondenceId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondenceReplyFile_FileManagerId",
                table: "LitigationRequestCorrespondenceReplyFile",
                column: "FileManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondenceReplyFile_LitigationRequestCorrespondenceReplyId",
                table: "LitigationRequestCorrespondenceReplyFile",
                column: "LitigationRequestCorrespondenceReplyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LitigationRequestCorrespondenceReplyFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestCorrespondenceReply");
        }
    }
}
