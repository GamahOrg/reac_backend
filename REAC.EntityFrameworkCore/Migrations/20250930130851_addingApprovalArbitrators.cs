using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class addingApprovalArbitrators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplainFiles");

            migrationBuilder.DropTable(
                name: "Complains");

            migrationBuilder.DropTable(
                name: "ComplainType");

            migrationBuilder.DropTable(
                name: "ComplainsStatuses");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArbitrationMinutesOn",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ResponseArbitrationMinutes",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArbitrationMinutesOn",
                table: "LitigationRequestArbitratorAssigningRequest");

            migrationBuilder.DropColumn(
                name: "ResponseArbitrationMinutes",
                table: "LitigationRequestArbitratorAssigningRequest");

            migrationBuilder.CreateTable(
                name: "ComplainsStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplainsStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComplainType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplainType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Complains",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComplainsStatusId = table.Column<int>(type: "int", nullable: false),
                    ComplainsTypeId = table.Column<int>(type: "int", nullable: false),
                    ApplicantCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedTime = table.Column<TimeSpan>(type: "time", nullable: false, defaultValueSql: "CONVERT(TIME, GETDATE())"),
                    CustomerNID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MobileNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SysFileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TicketDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Complains_ComplainType_ComplainsTypeId",
                        column: x => x.ComplainsTypeId,
                        principalTable: "ComplainType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complains_ComplainsStatuses_ComplainsStatusId",
                        column: x => x.ComplainsStatusId,
                        principalTable: "ComplainsStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complains_SysFileManager_SysFileManagerId",
                        column: x => x.SysFileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ComplainFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComplainId = table.Column<long>(type: "bigint", nullable: false),
                    FileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplainFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplainFiles_Complains_ComplainId",
                        column: x => x.ComplainId,
                        principalTable: "Complains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComplainFiles_SysFileManager_FileManagerId",
                        column: x => x.FileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplainFiles_ComplainId",
                table: "ComplainFiles",
                column: "ComplainId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplainFiles_FileManagerId",
                table: "ComplainFiles",
                column: "FileManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Complains_ComplainsStatusId",
                table: "Complains",
                column: "ComplainsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Complains_ComplainsTypeId",
                table: "Complains",
                column: "ComplainsTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Complains_SysFileManagerId",
                table: "Complains",
                column: "SysFileManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Complains_TicketNumber",
                table: "Complains",
                column: "TicketNumber",
                unique: true);
        }
    }
}
