using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddLitigationRequestCategoryToLitigationRequestNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequestNote",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestNote_LitigationRequestCategoryId",
                table: "LitigationRequestNote",
                column: "LitigationRequestCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitigationRequestNote_SysLitigationRequestCategory_LitigationRequestCategoryId",
                table: "LitigationRequestNote",
                column: "LitigationRequestCategoryId",
                principalTable: "SysLitigationRequestCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitigationRequestNote_SysLitigationRequestCategory_LitigationRequestCategoryId",
                table: "LitigationRequestNote");

            migrationBuilder.DropIndex(
                name: "IX_LitigationRequestNote_LitigationRequestCategoryId",
                table: "LitigationRequestNote");

            migrationBuilder.DropColumn(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequestNote");
        }
    }
}
