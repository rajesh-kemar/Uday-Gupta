using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemar.WHL.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddProductToPicking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Pickings");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Pickings",
                newName: "PickedQuantity");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_ProductId",
                table: "Pickings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_ShipmentId",
                table: "Pickings",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_WarehouseId",
                table: "Pickings",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pickings_Products_ProductId",
                table: "Pickings",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pickings_Shipments_ShipmentId",
                table: "Pickings",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "ShipmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pickings_Warehouses_WarehouseId",
                table: "Pickings",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pickings_Products_ProductId",
                table: "Pickings");

            migrationBuilder.DropForeignKey(
                name: "FK_Pickings_Shipments_ShipmentId",
                table: "Pickings");

            migrationBuilder.DropForeignKey(
                name: "FK_Pickings_Warehouses_WarehouseId",
                table: "Pickings");

            migrationBuilder.DropIndex(
                name: "IX_Pickings_ProductId",
                table: "Pickings");

            migrationBuilder.DropIndex(
                name: "IX_Pickings_ShipmentId",
                table: "Pickings");

            migrationBuilder.DropIndex(
                name: "IX_Pickings_WarehouseId",
                table: "Pickings");

            migrationBuilder.RenameColumn(
                name: "PickedQuantity",
                table: "Pickings",
                newName: "Quantity");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "Pickings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
