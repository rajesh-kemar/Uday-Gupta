using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemar.WHL.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addstatusinshipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Shipments");
        }
    }
}
