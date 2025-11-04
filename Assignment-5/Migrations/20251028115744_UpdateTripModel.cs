using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripApiEF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTripModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "Id", "Date", "Destination", "DriverId", "IsCompleted", "VehicleId" },
                values: new object[] { 1, new DateTime(2025, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "New York", 1, false, 1 });
        }
    }
}
