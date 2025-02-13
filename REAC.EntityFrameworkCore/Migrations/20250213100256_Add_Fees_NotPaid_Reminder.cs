using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Add_Fees_NotPaid_Reminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<bool>(
                name: "IsLitigationRequestFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true);

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "IsLitigationRequestFeesNotPaidFirstReminderSent",
                table: "LitigationRequest");

            
        }
    }
}
