using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddFlagsNominationList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefendantSelected",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlaintiffSelected",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsResponse",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefendantSelected",
                table: "LitigationRequestArbitratorNominationList");

            migrationBuilder.DropColumn(
                name: "IsPlaintiffSelected",
                table: "LitigationRequestArbitratorNominationList");

            migrationBuilder.DropColumn(
                name: "IsResponse",
                table: "LitigationRequestArbitratorNominationList");
        }
    }
}
