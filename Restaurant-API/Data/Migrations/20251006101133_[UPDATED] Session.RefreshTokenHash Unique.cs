using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UPDATEDSessionRefreshTokenHashUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RefreshTokenHash",
                table: "Sessions",
                column: "RefreshTokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_RefreshTokenHash",
                table: "Sessions");
        }
    }
}
