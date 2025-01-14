using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class newtableforcitizenrequestcvfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExperiencedInHousingstatuses",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ExperiencedInArbitrationFields",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CitizenRequestCVFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CVFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestCVFilesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRequestCVFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitizenRequestCVFile_CitizenRequest_CitizenRequestId",
                        column: x => x.CitizenRequestId,
                        principalTable: "CitizenRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenRequestCVFile_SysFileManager_CitizenRequestCVFilesId",
                        column: x => x.CitizenRequestCVFilesId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestCVFile_CitizenRequestCVFilesId",
                table: "CitizenRequestCVFile",
                column: "CitizenRequestCVFilesId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestCVFile_CitizenRequestId",
                table: "CitizenRequestCVFile",
                column: "CitizenRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitizenRequestCVFile");

            migrationBuilder.AlterColumn<string>(
                name: "ExperiencedInHousingstatuses",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExperiencedInArbitrationFields",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
