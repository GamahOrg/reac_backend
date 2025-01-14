using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddLitigationRequestCorrespondenceStatusAndCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<int>(
                name: "LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialCode",
                table: "LitigationRequestCorrespondence",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SysLitigationRequestCorrespondenceStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    EnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysLitigationRequestCorrespondenceStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondence_LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                column: "LitigationRequestCorrespondenceStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestCorrespondence_SysLitigationRequestCorrespondenceStatus_LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                column: "LitigationRequestCorrespondenceStatusId",
                principalTable: "SysLitigationRequestCorrespondenceStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestCorrespondence_SysLitigationRequestCorrespondenceStatus_LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropTable(
                name: "SysLitigationRequestCorrespondenceStatus");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequestCorrespondence_LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropColumn(
                name: "LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropColumn(
                name: "LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence");

            migrationBuilder.DropColumn(
                name: "SerialCode",
                table: "LitigationRequestCorrespondence");
        }
    }
}
