using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZonefyDotnet.Migrations
{
    /// <inheritdoc />
    public partial class addedIsBlockedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "PropertyStatistics",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "HouseProperties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PostCode",
                table: "HouseProperties",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "PropertyStatistics");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "HouseProperties");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "HouseProperties");
        }
    }
}
