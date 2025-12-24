import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';

import { PickingService, PickingRequest, PickingResponse } from '../../../core/services/picking.service';
import { ShipmentService, ShipmentRequest } from '../../../core/services/shipment.service';
import { InventoryService, InventoryResponse } from '../../../core/services/inventory.service';

@Component({
  selector: 'app-picking',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './picking.component.html',
  styleUrls: ['./picking.component.css']
})
export class PickingComponent implements OnInit {

  shipmentId!: number;
  shipmentStatus = '';

  pickingList: PickingResponse[] = [];
  inventories: InventoryResponse[] = [];
  products: { productId: number, productName: string }[] = [];

  warehouses: InventoryResponse[] = [];
  selectedProductId: number | null = null;
  selectedWarehouse: InventoryResponse | null = null;
  availableQuantity = 0;

  picking: PickingRequest = {
    shipmentId: 0,
    productId: 0,
    warehouseId: 0,
    pickedQuantity: 1
  };

  loadingTable = true;
  loadingProducts = true;
  loading = false;

  constructor(
    private route: ActivatedRoute,
    private pickingService: PickingService,
    private shipmentService: ShipmentService,
    private inventoryService: InventoryService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('shipmentId');
    if (!id) return;

    this.shipmentId = +id;
    this.picking.shipmentId = this.shipmentId;

    this.loadShipment();
    this.loadPicking();
    this.loadInventories();
  }

  loadShipment(): void {
    this.shipmentService.getAll().subscribe(res => {
      const shipment = res.find(s => s.shipmentId === this.shipmentId);
      if (shipment) {
        this.shipmentStatus = shipment.status;
        this.cdr.detectChanges();
      }
    });
  }

  loadPicking(): void {
    this.loadingTable = true;
    this.pickingService.getByShipment(this.shipmentId).subscribe(res => {
      this.pickingList = res || [];
      this.loadingTable = false;
      this.cdr.detectChanges();
    });
  }

  loadInventories(): void {
    this.loadingProducts = true;
    this.inventoryService.getAll().subscribe(res => {
      this.inventories = res || [];

      // Only keep products that have quantity > 0
      const productMap = new Map<number, string>();
      this.inventories.forEach(inv => {
        if (inv.quantity > 0 && !productMap.has(inv.productId)) {
          productMap.set(inv.productId, inv.productName);
        }
      });

      this.products = Array.from(productMap.entries()).map(([productId, productName]) => ({
        productId,
        productName
      }));

      this.selectedProductId = null;
      this.selectedWarehouse = null;
      this.availableQuantity = 0;
      this.warehouses = [];

      this.loadingProducts = false;
      this.cdr.detectChanges();
    });
  }

  onProductChange(productId: number | null): void {
    this.selectedProductId = productId;
    this.selectedWarehouse = null;
    this.availableQuantity = 0;

    if (!productId) {
      this.warehouses = [];
      this.cdr.detectChanges();
      return;
    }

    // Show only warehouses with available quantity
    this.warehouses = this.inventories.filter(inv => inv.productId === productId && inv.quantity > 0);
    this.cdr.detectChanges();
  }

  onWarehouseChange(inv: InventoryResponse | null): void {
    this.selectedWarehouse = inv;

    if (!inv) {
      this.availableQuantity = 0;
      this.picking.productId = 0;
      this.picking.warehouseId = 0;
      this.cdr.detectChanges();
      return;
    }

    this.availableQuantity = inv.quantity;
    this.picking.productId = inv.productId;
    this.picking.warehouseId = inv.warehouseId;
    this.picking.pickedQuantity = 1;

    this.cdr.detectChanges();
  }

  submitPick(): void {
    if (!this.picking.productId || !this.picking.warehouseId) {
      alert('Please select product and warehouse');
      return;
    }

    if (this.picking.pickedQuantity > this.availableQuantity) {
      alert(`Only ${this.availableQuantity} available`);
      return;
    }

    this.loading = true;

    this.pickingService.pick(this.picking).subscribe({
      next: () => {
        this.loading = false;
        this.resetPickForm();
        this.loadPicking();
        this.loadInventories();
        this.updateShipmentStatusToPicked();
      },
      error: err => {
        this.loading = false;
        alert(err.error?.message || 'Picking failed');
      }
    });
  }

  private resetPickForm(): void {
    this.picking.pickedQuantity = 1;
    this.availableQuantity = 0;
    this.selectedProductId = null;
    this.selectedWarehouse = null;
    this.warehouses = [];
  }

  private updateShipmentStatusToPicked(): void {
    if (this.shipmentStatus !== 'CREATED') return;

    const update: ShipmentRequest = {
      shipmentId: this.shipmentId,
      shipmentNumber: '',
      shipmentDate: new Date().toISOString(),
      vehicleId: 0,
      destinationId: 0,
      status: 'PICKED'
    };

    this.shipmentService.addOrUpdate(update).subscribe(() => {
      this.shipmentStatus = 'PICKED';
      this.cdr.detectChanges();
    });
  }
}
