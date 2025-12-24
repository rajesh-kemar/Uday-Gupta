using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemar.WHL.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddLastActivityTimeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityTime",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActivityTime",
                table: "Users");
        }
    }
}
