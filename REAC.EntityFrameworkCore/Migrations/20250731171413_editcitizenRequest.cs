using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class editcitizenRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExperiencedInFreelanceArbitration",
                table: "CitizenRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HaveExperiencedInFreelanceArbitration",
                table: "CitizenRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsregisteredWithBachelorShareaaDegree",
                table: "CitizenRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperiencedInFreelanceArbitration",
                table: "CitizenRequest");

            migrationBuilder.DropColumn(
                name: "HaveExperiencedInFreelanceArbitration",
                table: "CitizenRequest");

            migrationBuilder.DropColumn(
                name: "IsregisteredWithBachelorShareaaDegree",
                table: "CitizenRequest");
        }
    }
}
