using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class addingNewCitizenRequestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExperiencedInArbitrationFields",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExperiencedInHousingstatuses",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HaveArbitrationxperience",
                table: "CitizenRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HaveHostingExperience",
                table: "CitizenRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRegisteredReconciliationCenter",
                table: "CitizenRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IspassedRealEstateProgram",
                table: "CitizenRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CitizenRequestProgramRealEstateInstituteFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RealEstateInstituteFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FreelanceWorkDocumentFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRequestProgramRealEstateInstituteFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitizenRequestProgramRealEstateInstituteFiles_CitizenRequest_CitizenRequestId",
                        column: x => x.CitizenRequestId,
                        principalTable: "CitizenRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenRequestProgramRealEstateInstituteFiles_SysFileManager_FreelanceWorkDocumentFileId",
                        column: x => x.FreelanceWorkDocumentFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CitizenRequestReconciliationCenterDegreeFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReconciliationCenterDegreeFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FreelanceWorkDocumentFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRequestReconciliationCenterDegreeFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitizenRequestReconciliationCenterDegreeFiles_CitizenRequest_CitizenRequestId",
                        column: x => x.CitizenRequestId,
                        principalTable: "CitizenRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenRequestReconciliationCenterDegreeFiles_SysFileManager_FreelanceWorkDocumentFileId",
                        column: x => x.FreelanceWorkDocumentFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestProgramRealEstateInstituteFiles_CitizenRequestId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "CitizenRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestProgramRealEstateInstituteFiles_FreelanceWorkDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "FreelanceWorkDocumentFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestReconciliationCenterDegreeFiles_CitizenRequestId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "CitizenRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestReconciliationCenterDegreeFiles_FreelanceWorkDocumentFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "FreelanceWorkDocumentFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropTable(
                name: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropColumn(
                name: "ExperiencedInArbitrationFields",
                table: "CitizenRequest");

            migrationBuilder.DropColumn(
                name: "ExperiencedInHousingstatuses",
                table: "CitizenRequest");

            migrationBuilder.DropColumn(
                name: "HaveArbitrationxperience",
                table: "CitizenRequest");

            migrationBuilder.DropColumn(
                name: "HaveHostingExperience",
                table: "CitizenRequest");

            migrationBuilder.DropColumn(
                name: "IsRegisteredReconciliationCenter",
                table: "CitizenRequest");

            migrationBuilder.DropColumn(
                name: "IspassedRealEstateProgram",
                table: "CitizenRequest");
        }
    }
}
