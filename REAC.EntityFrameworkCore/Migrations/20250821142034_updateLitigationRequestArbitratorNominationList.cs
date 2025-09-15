using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class updateLitigationRequestArbitratorNominationList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EscalateDate",
                table: "LitigationRequestArbitratorNominationList",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEscalated",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelectedFromManager",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EscalateDate",
                table: "LitigationRequestArbitratorNominationList");

            migrationBuilder.DropColumn(
                name: "IsEscalated",
                table: "LitigationRequestArbitratorNominationList");

            migrationBuilder.DropColumn(
                name: "IsSelectedFromManager",
                table: "LitigationRequestArbitratorNominationList");
        }
    }
}
