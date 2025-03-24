using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Alter_LitigationRequest_Add_IsArbitratorSessionTimeAddedPeriodSent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArbitratorSessionTimeAddedPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArbitratorSessionTimeAddedPeriodSent",
                table: "LitigationRequest");
        }
    }
}
