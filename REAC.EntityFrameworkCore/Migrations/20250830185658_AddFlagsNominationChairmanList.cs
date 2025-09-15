using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddFlagsNominationChairmanList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EscalateDate",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEscalated",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelectedFromManager",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Response",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EscalateDate",
                table: "LitigationRequestArbitratorsChairmanNominationList");

            migrationBuilder.DropColumn(
                name: "IsEscalated",
                table: "LitigationRequestArbitratorsChairmanNominationList");

            migrationBuilder.DropColumn(
                name: "IsSelectedFromManager",
                table: "LitigationRequestArbitratorsChairmanNominationList");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "LitigationRequestArbitratorsChairmanNominationList");
        }
    }
}
