using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationBetweenASPNETUserAndBookAndMemberEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Memberships",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookCreatorId",
                table: "Books",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_UserId",
                table: "Memberships",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookCreatorId",
                table: "Books",
                column: "BookCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_BookCreatorId",
                table: "Books",
                column: "BookCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_AspNetUsers_UserId",
                table: "Memberships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_BookCreatorId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_AspNetUsers_UserId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_UserId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookCreatorId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "BookCreatorId",
                table: "Books");
        }
    }
}
