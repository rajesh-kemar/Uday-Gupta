import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../shared/navbar/navbar.component';
import {
  VehicleService,
  VehicleRequest,
  VehicleResponse,
  VehicleFilterRequest
} from '../../core/services/vehicle.service';

@Component({
  selector: 'app-vehicle',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.css']
})
export class VehicleComponent implements OnInit {

  userName = '';
  roles: string[] = [];

  allVehicles: VehicleResponse[] = [];
  vehicles: VehicleResponse[] = [];
  originalVehicles: VehicleResponse[] = []; // keep original list

  searchText = '';
  searchBy: 'vehicleNumber' | 'type' | 'status' = 'vehicleNumber';
  loadingVehicles = true;

  showAddForm = false;
  isEditMode = false;
  isSubmitting = false;

  formError = false;
  formMessage = '';

  page = 1;
  pageSize = 5;
  pageSizeOptions = [5, 10, 20];

  vehicle: VehicleRequest = {
    vehicleId: undefined,
    vehicleNumber: '',
    type: '',
    capacity: 0,
    isActive: true
  };

  constructor(
    private vehicleService: VehicleService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadUser();
    this.loadVehicles();
  }

  private loadUser(): void {
    this.userName = localStorage.getItem('userName') || 'User';
    const rolesStr = localStorage.getItem('roles');
    this.roles = rolesStr ? JSON.parse(rolesStr) : [];
  }

  loadVehicles(): void {
    this.loadingVehicles = true;
    this.vehicleService.getAll().subscribe(res => {
      this.allVehicles = res.map(v => ({ ...v, type: v.type || '' }));
      this.originalVehicles = [...this.allVehicles]; // save original list
      this.page = 1;
      this.applyPagination();
      this.loadingVehicles = false;
      this.cdr.detectChanges();
    });
  }

  searchVehicles(): void {
    const text = this.searchText.trim().toLowerCase();

    if (!text) {
      // Empty search → restore original list
      this.allVehicles = [...this.originalVehicles];
      this.page = 1;
      this.applyPagination();
      return;
    }

    let filtered: VehicleResponse[] = [];

    switch (this.searchBy) {
      case 'vehicleNumber':
        filtered = this.originalVehicles.filter(v =>
          v.vehicleNumber.toLowerCase().includes(text)
        );
        break;
      case 'type':
        filtered = this.originalVehicles.filter(v =>
          v.type.toLowerCase().includes(text)
        );
        break;
      case 'status':
        if (text === 'active') filtered = this.originalVehicles.filter(v => v.isActive);
        else if (text === 'inactive') filtered = this.originalVehicles.filter(v => !v.isActive);
        else filtered = [...this.originalVehicles];
        break;
    }

    this.allVehicles = filtered;
    this.page = 1;
    this.applyPagination();
  }

  applyPagination(): void {
    const start = (this.page - 1) * this.pageSize;
    this.vehicles = this.allVehicles.slice(start, start + this.pageSize);
  }

  nextPage(): void {
    if (this.page * this.pageSize < this.allVehicles.length) {
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

  openAdd(): void {
    this.isEditMode = false;
    this.formError = false;
    this.vehicle = {
      vehicleId: undefined,
      vehicleNumber: '',
      type: '',
      capacity: 0,
      isActive: true
    };
    this.showAddForm = true;
  }

  editVehicle(v: VehicleResponse): void {
    this.isEditMode = true;
    this.formError = false;
    this.vehicle = { 
      vehicleId: v.vehicleId, 
      vehicleNumber: v.vehicleNumber, 
      type: v.type || '', 
      capacity: v.capacity, 
      isActive: v.isActive 
    };
    this.showAddForm = true;
  }

  closeForm(): void {
    if (!this.isSubmitting) this.showAddForm = false;
  }

  private validateForm(): boolean {
    if (!this.vehicle.vehicleNumber?.trim())
      return this.setError('Vehicle Number required');
    if (!this.vehicle.type?.trim())
      return this.setError('Vehicle Type required');
    if (!this.vehicle.capacity || this.vehicle.capacity <= 0)
      return this.setError('Capacity must be greater than 0');
    return true;
  }

  private setError(msg: string): boolean {
    this.formError = true;
    this.formMessage = msg;
    return false;
  }

  submitForm(): void {
    if (!this.validateForm()) return;

    this.isSubmitting = true;
    this.vehicleService.addOrUpdate(this.vehicle).subscribe(() => {
      this.isSubmitting = false;
      this.showAddForm = false;
      this.loadVehicles();
    });
  }

  


  deleteVehicle(id: number): void {
    if (!confirm('Delete this vehicle?')) return;
    this.vehicleService.delete(id).subscribe(() => {
      this.loadVehicles();
    });
  }
}
