using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ADDEDIsCreatedByStaffinReservationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedByStaff",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreatedByStaff",
                table: "Reservations");
        }
    }
}
