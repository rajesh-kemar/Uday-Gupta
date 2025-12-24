import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../shared/navbar/navbar.component';

import {
  InventoryService,
  InventoryRequest,
  InventoryResponse
} from '../../core/services/inventory.service';

import {
  WarehouseService,
  WarehouseResponse
} from '../../core/services/warehouse.service';

import {
  ProductService,
  ProductResponse
} from '../../core/services/product.service';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './inventory.component.html',
  styleUrls: ['./inventory.component.css']
})
export class InventoryComponent implements OnInit {

  userName = '';
  roles: string[] = [];

  inventories: InventoryResponse[] = [];
  originalInventories: InventoryResponse[] = [];

  warehouses: WarehouseResponse[] = [];
  products: ProductResponse[] = [];

  searchText = '';
  loadingInventories = true;

  showAddForm = false;
  isEditMode = false;
  isSubmitting = false;

  // Pagination
  page = 1;
  pageSize = 5;
  pageSizeOptions = [5, 10, 20];
  pagedInventories: InventoryResponse[] = [];

  inventory: InventoryRequest = {
    quantity: 0,
    warehouseId: 0,
    productId: 0
  };

  constructor(
    private inventoryService: InventoryService,
    private warehouseService: WarehouseService,
    private productService: ProductService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadUser();
    this.loadWarehouses();
    this.loadInventories();
  }

  // ================= USER =================
  private loadUser(): void {
    this.userName = localStorage.getItem('userName') || 'User';
    const rolesStr = localStorage.getItem('roles');
    this.roles = rolesStr ? JSON.parse(rolesStr) : [];
  }

  // ================= WAREHOUSE =================
  private loadWarehouses(): void {
    this.warehouseService.getAll().subscribe(res => {
      const allWarehouses = res || [];

      if (this.roles.includes('WarehouseStaff')) {
        const wid = Number(localStorage.getItem('warehouseId'));

        if (!wid) {
          alert('Warehouse not assigned to this user');
          this.warehouses = [];
          return;
        }

        this.warehouses = allWarehouses.filter(w => w.warehouseId === wid);
      } else {
        // Admin / Manager
        this.warehouses = allWarehouses;
      }

      if (this.warehouses.length > 0) {
        this.inventory.warehouseId = this.warehouses[0].warehouseId;
        this.loadProducts(this.inventory.warehouseId);
      }
    });
  }

  onWarehouseChange(event: Event): void {
    const warehouseId = Number((event.target as HTMLSelectElement).value);
    this.inventory.warehouseId = warehouseId;
    this.loadProducts(warehouseId);
  }

  // ================= PRODUCTS =================
  loadProducts(warehouseId: number): void {

    // WarehouseStaff → warehouse-based products
    if (this.roles.includes('WarehouseStaff')) {
      this.productService.getByWarehouse(warehouseId).subscribe(res => {
        this.products = res || [];
        this.inventory.productId = this.products.length
          ? this.products[0].productId
          : 0;
        this.cdr.detectChanges();
      });
    }
    // Admin → all products
    else {
      this.productService.getAll().subscribe(res => {
        this.products = res || [];
        this.inventory.productId = this.products.length
          ? this.products[0].productId
          : 0;
        this.cdr.detectChanges();
      });
    }
  }

  // ================= INVENTORY =================
  loadInventories(): void {
    this.loadingInventories = true;
    this.inventoryService.getAll().subscribe(res => {
      this.inventories = res || [];
      this.originalInventories = [...this.inventories];
      this.page = 1;
      this.applyPagination();
      this.loadingInventories = false;
      this.cdr.detectChanges();
    });
  }

  searchInventories(): void {
    const text = this.searchText.trim().toLowerCase();

    if (!text) {
      this.inventories = [...this.originalInventories];
    } else {
      this.inventories = this.originalInventories.filter(inv =>
        inv.warehouseName.toLowerCase().includes(text) ||
        inv.productName.toLowerCase().includes(text)
      );
    }

    this.page = 1;
    this.applyPagination();
  }

  // ================= PAGINATION =================
  applyPagination(): void {
    const start = (this.page - 1) * this.pageSize;
    this.pagedInventories = this.inventories.slice(start, start + this.pageSize);
  }

  nextPage(): void {
    if (this.page * this.pageSize < this.inventories.length) {
      this.page++;
      this.applyPagination();
    }
  }

  prevPage(): void {
    if (this.page > 1) {
      this.page--;
      this.applyPagination();
    }
  }

  changePageSize(): void {
    this.page = 1;
    this.applyPagination();
  }

  // ================= FORM =================
  openAdd(): void {
    if (!this.inventory.warehouseId) {
      alert('Please select a warehouse first');
      return;
    }

    this.isEditMode = false;
    this.inventory = {
      quantity: 0,
      warehouseId: this.inventory.warehouseId,
      productId: 0
    };

    this.showAddForm = true;
    this.loadProducts(this.inventory.warehouseId);
  }

  editInventory(inv: InventoryResponse): void {
    this.isEditMode = true;
    this.inventory = {
      inventoryId: inv.inventoryId,
      quantity: inv.quantity,
      warehouseId: inv.warehouseId,
      productId: inv.productId
    };
    this.showAddForm = true;
    this.loadProducts(inv.warehouseId);
  }

  submitForm(): void {
    if (
      this.inventory.quantity <= 0 ||
      !this.inventory.warehouseId ||
      !this.inventory.productId
    ) {
      alert('All fields required & quantity must be greater than 0');
      return;
    }

    const existing = this.originalInventories.find(
      x =>
        x.warehouseId === this.inventory.warehouseId &&
        x.productId === this.inventory.productId
    );

    if (existing && !this.isEditMode) {
      this.inventory.inventoryId = existing.inventoryId;
    }

    this.isSubmitting = true;
    this.inventoryService.addOrUpdate(this.inventory).subscribe(() => {
      this.isSubmitting = false;
      this.showAddForm = false;
      this.loadInventories();
    });
  }

  deleteInventory(id: number): void {
    if (!confirm('Delete this inventory?')) return;
    this.inventoryService.delete(id).subscribe(() => this.loadInventories());
  }

  closeForm(): void {
    this.showAddForm = false;
  }
}