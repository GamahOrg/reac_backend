using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonToLitigationRequestReformerAssigningRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestCVFile_CitizenRequest_CitizenRequestId",
                table: "CitizenRequestCVFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestProgramRealEstateInstituteFiles_CitizenRequest_CitizenRequestId",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestProgramRealEstateInstituteFiles_SysFileManager_ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestReconciliationCenterDegreeFiles_CitizenRequest_CitizenRequestId",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestReconciliationCenterDegreeFiles_SysFileManager_ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitizenRequestReconciliationCenterDegreeFiles",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropIndex(
                name: "IX_CitizenRequestReconciliationCenterDegreeFiles_ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitizenRequestProgramRealEstateInstituteFiles",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropIndex(
                name: "IX_CitizenRequestProgramRealEstateInstituteFiles_ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitizenRequestCVFile",
                table: "CitizenRequestCVFile");

            migrationBuilder.DropColumn(
                name: "ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropColumn(
                name: "ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "LitigationRequestReformerAssigningRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitizenRequestReconciliationCenterDegreeFile3214EC0743341C99",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitizenRequestProgramRealEstateInstituteFile3214EC0743341C99",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitizenRequestCVFile3214EC0743341C99",
                table: "CitizenRequestCVFile",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestReconciliationCenterDegreeFiles_ReconciliationCenterDegreeFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "ReconciliationCenterDegreeFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestProgramRealEstateInstituteFiles_RealEstateInstituteFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "RealEstateInstituteFileId");

            migrationBuilder.AddForeignKey(
                name: "FKCitizenRequestCVFileCitizenRequest676101",
                table: "CitizenRequestCVFile",
                column: "CitizenRequestId",
                principalTable: "CitizenRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "CitizenRequestProgramRealEstateInstitutes616003",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "RealEstateInstituteFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FKCitizenRequestProgramRealEstateCitizenRequest676101",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "CitizenRequestId",
                principalTable: "CitizenRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "CitizenRequestReconciliationCenterDegrees616003",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "ReconciliationCenterDegreeFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FKCitizenRequestReconciliationCenterDegreeFilesCitizenRequest676101",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "CitizenRequestId",
                principalTable: "CitizenRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FKCitizenRequestCVFileCitizenRequest676101",
                table: "CitizenRequestCVFile");

            migrationBuilder.DropForeignKey(
                name: "CitizenRequestProgramRealEstateInstitutes616003",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropForeignKey(
                name: "FKCitizenRequestProgramRealEstateCitizenRequest676101",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropForeignKey(
                name: "CitizenRequestReconciliationCenterDegrees616003",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropForeignKey(
                name: "FKCitizenRequestReconciliationCenterDegreeFilesCitizenRequest676101",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitizenRequestReconciliationCenterDegreeFile3214EC0743341C99",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropIndex(
                name: "IX_CitizenRequestReconciliationCenterDegreeFiles_ReconciliationCenterDegreeFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitizenRequestProgramRealEstateInstituteFile3214EC0743341C99",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropIndex(
                name: "IX_CitizenRequestProgramRealEstateInstituteFiles_RealEstateInstituteFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitizenRequestCVFile3214EC0743341C99",
                table: "CitizenRequestCVFile");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "LitigationRequestReformerAssigningRequest");

            migrationBuilder.AddColumn<Guid>(
                name: "ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitizenRequestReconciliationCenterDegreeFiles",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitizenRequestProgramRealEstateInstituteFiles",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitizenRequestCVFile",
                table: "CitizenRequestCVFile",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestReconciliationCenterDegreeFiles_ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "ReconciliationCenterFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestProgramRealEstateInstituteFiles_ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "ProgramRealEstateDocumentFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestCVFile_CitizenRequest_CitizenRequestId",
                table: "CitizenRequestCVFile",
                column: "CitizenRequestId",
                principalTable: "CitizenRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestProgramRealEstateInstituteFiles_CitizenRequest_CitizenRequestId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "CitizenRequestId",
                principalTable: "CitizenRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestProgramRealEstateInstituteFiles_SysFileManager_ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "ProgramRealEstateDocumentFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestReconciliationCenterDegreeFiles_CitizenRequest_CitizenRequestId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "CitizenRequestId",
                principalTable: "CitizenRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestReconciliationCenterDegreeFiles_SysFileManager_ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "ReconciliationCenterFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");
        }
    }
}
