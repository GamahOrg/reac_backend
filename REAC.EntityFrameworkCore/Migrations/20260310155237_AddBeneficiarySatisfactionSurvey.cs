using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddBeneficiarySatisfactionSurvey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SessionTypeId",
                table: "LitigationRequestSession",
                type: "int",
                nullable: true,
                comment: "Type of the session (ArbitrationSession, VerdictPronouncement, DeliberationBetweenArbitrators)");

            migrationBuilder.AddColumn<int>(
                name: "ReconciliationRequestDecision",
                table: "LitigationRequestReconciliationReport",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "serviceId",
                table: "LitigationRequestInvoice",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArbitratorAdminSelected",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BeneficiarySatisfactionSurvey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to LitigationRequest"),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "FK to Citizen who submitted the survey"),
                    StageType = table.Column<int>(type: "int", nullable: false, comment: "1 = Arbitration, 2 = Reconciliation"),
                    ProcedureSatisfaction = table.Column<int>(type: "int", nullable: true, comment: "Q1: Satisfaction with procedures (1-5), null if dismissed"),
                    PerformanceSatisfaction = table.Column<int>(type: "int", nullable: true, comment: "Q2: Satisfaction with arbitrator/reformer performance (1-5), null if dismissed"),
                    IsRated = table.Column<bool>(type: "bit", nullable: false, comment: "True if user submitted ratings, false if dismissed without rating"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BeneficiarySatisfactionSurvey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeneficiarySatisfactionSurvey_Citizen",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BeneficiarySatisfactionSurvey_LitigationRequest",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ComplaintFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintFile_SysFileManager_FileManagerId",
                        column: x => x.FileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiarySatisfactionSurvey_CitizenId",
                table: "BeneficiarySatisfactionSurvey",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiarySatisfactionSurvey_Unique",
                table: "BeneficiarySatisfactionSurvey",
                columns: new[] { "LitigationRequestId", "CitizenId", "StageType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintFile_FileManagerId",
                table: "ComplaintFile",
                column: "FileManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeneficiarySatisfactionSurvey");

            migrationBuilder.DropTable(
                name: "ComplaintFile");

            migrationBuilder.DropColumn(
                name: "SessionTypeId",
                table: "LitigationRequestSession");

            migrationBuilder.DropColumn(
                name: "ReconciliationRequestDecision",
                table: "LitigationRequestReconciliationReport");

            migrationBuilder.DropColumn(
                name: "serviceId",
                table: "LitigationRequestInvoice");

            migrationBuilder.DropColumn(
                name: "IsArbitratorAdminSelected",
                table: "LitigationRequestArbitratorNominationList");
        }
    }
}
