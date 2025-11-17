using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UPDATEDReservationhasStartTimeUtcRestauranthasTimeZoneId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Restaurants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTimeUtc",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "StartTimeUtc",
                table: "Reservations");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Reservations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Time",
                table: "Reservations",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
