using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class newrelationcitizenrequestfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestProgramRealEstateInstituteFiles_SysFileManager_FreelanceWorkDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestReconciliationCenterDegreeFiles_SysFileManager_FreelanceWorkDocumentFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.RenameColumn(
                name: "FreelanceWorkDocumentFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                newName: "ReconciliationCenterFileId");

            migrationBuilder.RenameIndex(
                name: "IX_CitizenRequestReconciliationCenterDegreeFiles_FreelanceWorkDocumentFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                newName: "IX_CitizenRequestReconciliationCenterDegreeFiles_ReconciliationCenterFileId");

            migrationBuilder.RenameColumn(
                name: "FreelanceWorkDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                newName: "ProgramRealEstateDocumentFileId");

            migrationBuilder.RenameIndex(
                name: "IX_CitizenRequestProgramRealEstateInstituteFiles_FreelanceWorkDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                newName: "IX_CitizenRequestProgramRealEstateInstituteFiles_ProgramRealEstateDocumentFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestProgramRealEstateInstituteFiles_SysFileManager_ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "ProgramRealEstateDocumentFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestReconciliationCenterDegreeFiles_SysFileManager_ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "ReconciliationCenterFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestProgramRealEstateInstituteFiles_SysFileManager_ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CitizenRequestReconciliationCenterDegreeFiles_SysFileManager_ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles");

            migrationBuilder.RenameColumn(
                name: "ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                newName: "FreelanceWorkDocumentFileId");

            migrationBuilder.RenameIndex(
                name: "IX_CitizenRequestReconciliationCenterDegreeFiles_ReconciliationCenterFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                newName: "IX_CitizenRequestReconciliationCenterDegreeFiles_FreelanceWorkDocumentFileId");

            migrationBuilder.RenameColumn(
                name: "ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                newName: "FreelanceWorkDocumentFileId");

            migrationBuilder.RenameIndex(
                name: "IX_CitizenRequestProgramRealEstateInstituteFiles_ProgramRealEstateDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                newName: "IX_CitizenRequestProgramRealEstateInstituteFiles_FreelanceWorkDocumentFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestProgramRealEstateInstituteFiles_SysFileManager_FreelanceWorkDocumentFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                column: "FreelanceWorkDocumentFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenRequestReconciliationCenterDegreeFiles_SysFileManager_FreelanceWorkDocumentFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                column: "FreelanceWorkDocumentFileId",
                principalTable: "SysFileManager",
                principalColumn: "Id");
        }
    }
}
