using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZonefyDotnet.Migrations
{
    /// <inheritdoc />
    public partial class ChangePropMessageChatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerifiedAt",
                table: "Users",
                newName: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "Users",
                newName: "VerifiedAt");
        }
    }
}
