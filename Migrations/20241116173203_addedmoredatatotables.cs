using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZonefyDotnet.Migrations
{
    /// <inheritdoc />
    public partial class addedmoredatatotables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyName",
                table: "PropertyStatistics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTime",
                table: "HouseProperties",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutTime",
                table: "HouseProperties",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Dimension",
                table: "HouseProperties",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Guests",
                table: "HouseProperties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "HouseProperties",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyName",
                table: "PropertyStatistics");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "HouseProperties");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "HouseProperties");

            migrationBuilder.DropColumn(
                name: "Dimension",
                table: "HouseProperties");

            migrationBuilder.DropColumn(
                name: "Guests",
                table: "HouseProperties");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "HouseProperties");
        }
    }
}
