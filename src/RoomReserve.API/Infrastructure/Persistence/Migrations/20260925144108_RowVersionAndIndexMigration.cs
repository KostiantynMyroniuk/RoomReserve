using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomReserve.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RowVersionAndIndexMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ConferenceRooms",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId_Date",
                table: "Bookings",
                columns: new[] { "RoomId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId_Date",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ConferenceRooms");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");
        }
    }
}
