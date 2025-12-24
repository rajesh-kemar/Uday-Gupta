import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../shared/navbar/navbar.component';
import {
  WarehouseService,
  WarehouseRequest,
  WarehouseResponse
} from '../../core/services/warehouse.service';

@Component({
  selector: 'app-warehouse',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.css']
})
export class WarehouseComponent implements OnInit {

  userName = '';
  roles: string[] = [];

  allWarehouses: WarehouseResponse[] = [];
  warehouses: WarehouseResponse[] = [];
  originalWarehouses: WarehouseResponse[] = [];

  searchText = '';
  searchBy: 'name' | 'location' = 'name';
  loadingWarehouses = true;

  showAddForm = false;
  isEditMode = false;
  isSubmitting = false;

  page = 1;
  pageSize = 5;
  pageSizeOptions = [5, 10, 20];

  warehouse: WarehouseRequest = { name: '', location: '', capacity: 0 };

  constructor(
    private warehouseService: WarehouseService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadUserFromLocalStorage();
    this.loadWarehouses();
  }

  private loadUserFromLocalStorage(): void {
    this.userName = localStorage.getItem('userName') || 'User';
    const rolesStr = localStorage.getItem('roles');
    this.roles = rolesStr ? JSON.parse(rolesStr) : [];
  }

  // Load all warehouses
  loadWarehouses(): void {
    this.loadingWarehouses = true;
    this.warehouseService.getAll().subscribe(res => {
      this.allWarehouses = res || [];
      this.originalWarehouses = [...this.allWarehouses];
      this.page = 1;
      this.applyPagination();
      this.loadingWarehouses = false;
      this.cdr.detectChanges();
    });
  }

  // Search/filter warehouses
  searchWarehouses(): void {
    const text = this.searchText.trim().toLowerCase();
    if (!text) {
      this.allWarehouses = [...this.originalWarehouses];
      this.page = 1;
      this.applyPagination();
      return;
    }

    let filtered: WarehouseResponse[] = [];
    switch (this.searchBy) {
      case 'name':
        filtered = this.originalWarehouses.filter(w => w.name.toLowerCase().includes(text));
        break;
      case 'location':
        filtered = this.originalWarehouses.filter(w => w.location.toLowerCase().includes(text));
        break;
    }

    this.allWarehouses = filtered;
    this.page = 1;
    this.applyPagination();
  }

  // Pagination logic
  applyPagination(): void {
    const start = (this.page - 1) * this.pageSize;
    this.warehouses = this.allWarehouses.slice(start, start + this.pageSize);
  }

  nextPage(): void {
    if (this.page * this.pageSize < this.allWarehouses.length) {
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

  // Getter for total pages
  get totalPages(): number[] {
    const pages = Math.ceil(this.allWarehouses.length / this.pageSize);
    return Array.from({ length: pages }, (_, i) => i + 1);
  }

  // Modal actions
  openAdd(): void {
    this.isEditMode = false;
    this.warehouse = { name: '', location: '', capacity: 0 };
    this.showAddForm = true;
  }

  editWarehouse(w: WarehouseResponse): void {
    this.isEditMode = true;
    this.warehouse = { ...w };
    this.showAddForm = true;
  }

  closeForm(): void {
    if (!this.isSubmitting) this.showAddForm = false;
  }

  submitForm(): void {
    if (!this.warehouse.name?.trim() || !this.warehouse.location?.trim() || this.warehouse.capacity <= 0) {
      alert('Name, Location & Capacity are required');
      return;
    }

    this.isSubmitting = true;
    this.warehouseService.addOrUpdate(this.warehouse).subscribe(() => {
      this.isSubmitting = false;
      this.showAddForm = false;
      this.loadWarehouses();
    });
  }

  deleteWarehouse(id: number): void {
    if (!confirm('Delete this warehouse?')) return;
    this.warehouseService.delete(id).subscribe(() => {
      this.loadWarehouses();
    });
  }
}
