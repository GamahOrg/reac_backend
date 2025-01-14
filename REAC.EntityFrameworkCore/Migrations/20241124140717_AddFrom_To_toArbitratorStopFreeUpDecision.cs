using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddFrom_To_toArbitratorStopFreeUpDecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionFrom",
                table: "ArbitratorStopFreeUpDecision",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionTo",
                table: "ArbitratorStopFreeUpDecision",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecisionFrom",
                table: "ArbitratorStopFreeUpDecision");

            migrationBuilder.DropColumn(
                name: "DecisionTo",
                table: "ArbitratorStopFreeUpDecision");
        }
    }
}
