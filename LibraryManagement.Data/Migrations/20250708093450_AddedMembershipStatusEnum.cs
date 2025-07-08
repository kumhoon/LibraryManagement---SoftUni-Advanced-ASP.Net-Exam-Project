using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMembershipStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Memberships");
        }
    }
}
