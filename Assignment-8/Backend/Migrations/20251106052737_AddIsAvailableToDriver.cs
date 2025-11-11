using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvLogisticSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAvailableToDriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Drivers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Drivers");
        }
    }
}
