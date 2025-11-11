using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvLogisticSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddExperienceToDriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Experience",
                table: "Drivers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Drivers");
        }
    }
}
