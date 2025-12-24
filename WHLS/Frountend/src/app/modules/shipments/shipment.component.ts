import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { NavbarComponent } from '../../shared/navbar/navbar.component';

import {
  ShipmentService,
  ShipmentRequest,
  ShipmentResponse
} from '../../core/services/shipment.service';
import { VehicleService, VehicleResponse } from '../../core/services/vehicle.service';
import { DistanceService, DistanceResponse } from '../../core/services/distance.service';

@Component({
  selector: 'app-shipment',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, NavbarComponent],
  templateUrl: './shipment.component.html',
  styleUrls: ['./shipment.component.css']
})
export class ShipmentComponent implements OnInit {

  userName = '';
  roles: string[] = [];
  isAdmin = false;
  isWarehouse = false;

  shipments: ShipmentResponse[] = [];
  originalShipments: ShipmentResponse[] = [];
  pagedShipments: ShipmentResponse[] = [];

  vehicles: VehicleResponse[] = [];
  distances: DistanceResponse[] = [];

  statuses = ['CREATED', 'PICKED', 'SHIPPED'];

  loadingShipments = true;   // ✅ FIX
  showForm = false;
  isEdit = false;
  isSubmitting = false;

  shipment: ShipmentRequest = this.emptyShipment();

  searchText = '';
  searchBy: 'shipmentNumber' | 'status' | 'destination' = 'shipmentNumber';

  page = 1;
  pageSize = 5;
  pageSizeOptions = [5, 10, 20];

  constructor(
    private shipmentService: ShipmentService,
    private vehicleService: VehicleService,
    private distanceService: DistanceService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadUser();
    this.loadShipments();
  }

  loadUser() {
    this.userName = localStorage.getItem('userName') || 'User';
    const r = localStorage.getItem('roles');
    this.roles = r ? JSON.parse(r) : [];
    this.isAdmin = this.roles.includes('Admin');
    this.isWarehouse = this.roles.includes('WarehouseStaff') || this.roles.includes('Warehouse');
  }

  loadShipments() {
    this.loadingShipments = true;

    this.shipmentService.getAll().subscribe({
      next: res => {
        this.shipments = res ?? [];
        this.originalShipments = [...this.shipments];
        this.page = 1;
        this.applyPagination();

        this.loadingShipments = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loadingShipments = false;
        alert('Failed to load shipments');
      }
    });

    this.loadVehicles();
    this.loadDistances();
  }

  loadVehicles() {
    this.vehicleService.getAll().subscribe(res => {
      this.vehicles = this.isEdit && this.shipment.vehicleId
        ? res.filter(v => v.isActive || v.vehicleId === this.shipment.vehicleId)
        : res.filter(v => v.isActive);
    });
  }

  loadDistances() {
    this.distanceService.getAll().subscribe(res => this.distances = res);
  }

  emptyShipment(): ShipmentRequest {
    return {
      shipmentNumber: this.generateShipmentNumber(),
      shipmentDate: new Date().toISOString().substring(0, 10),
      destinationId: 0,
      vehicleId: 0,
      status: 'CREATED'
    };
  }

  generateShipmentNumber(): string {
    const d = new Date().toISOString().slice(2, 10).replace(/-/g, '');
    const r = Math.floor(Math.random() * 1000).toString().padStart(3, '0');
    return `SHIP-${d}-${r}`;
  }

  openAdd() {
    this.isEdit = false;
    this.shipment = this.emptyShipment();
    this.showForm = true;
  }

  editShipment(s: ShipmentResponse) {
    this.isEdit = true;
    this.shipment = {
      shipmentId: s.shipmentId,
      shipmentNumber: s.shipmentNumber,
      shipmentDate: s.shipmentDate.substring(0, 10),
      destinationId: s.destinationId,
      vehicleId: s.vehicleId,
      status: s.status
    };
    this.showForm = true;
  }

  submitForm() {
    if (!this.shipment.vehicleId || !this.shipment.destinationId) {
      alert('All fields required');
      return;
    }

    this.isSubmitting = true;
    this.shipmentService.addOrUpdate(this.shipment).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.showForm = false;
        this.loadShipments();
      },
      error: () => {
        this.isSubmitting = false;
        alert('Save failed');
      }
    });
  }

  deleteShipment(id: number) {
    if (!this.isAdmin || !confirm('Delete shipment?')) return;
    this.shipmentService.delete(id).subscribe(() => this.loadShipments());
  }

  searchShipments() {
    const t = this.searchText.trim().toLowerCase();

    this.shipments = !t
      ? [...this.originalShipments]
      : this.originalShipments.filter(s =>
          this.searchBy === 'shipmentNumber'
            ? s.shipmentNumber.toLowerCase().includes(t)
            : this.searchBy === 'status'
            ? s.status.toLowerCase().includes(t)
            : s.destinationAddress?.toLowerCase().includes(t)
        );

    this.page = 1;
    this.applyPagination();
  }

  applyPagination() {
    const start = (this.page - 1) * this.pageSize;
    this.pagedShipments = this.shipments.slice(start, start + this.pageSize);
  }

  nextPage() {
    if (this.page * this.pageSize < this.shipments.length) {
      this.page++;
      this.applyPagination();
    }
  }

  prevPage() {
    if (this.page > 1) {
      this.page--;
      this.applyPagination();
    }
  }

  changePageSize() {
    this.page = 1;
    this.applyPagination();
  }

  get pageNumbers(): number[] {
    return Array(Math.ceil(this.shipments.length / this.pageSize))
      .fill(0)
      .map((_, i) => i + 1);
  }

  goToPicking(shipmentId: number) {
    this.router.navigate([`/inventory/picking/${shipmentId}`]);
  }
}
