import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../shared/navbar/navbar.component';
import {
  DistanceService,
  DistanceRequest,
  DistanceResponse,
  DistanceFilterModel
} from '../../core/services/distance.service';

@Component({
  selector: 'app-distance',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './distance.component.html',      
  styleUrls: ['./distance.component.css']
})
export class DistanceComponent implements OnInit {

  userName = '';
  roles: string[] = [];

  allDistances: DistanceResponse[] = [];
  distances: DistanceResponse[] = [];
  originalDistances: DistanceResponse[] = [];

  searchText = '';
  showAddForm = false;
  isEditMode = false;
  isSubmitting = false;
  loadingDistances = true;
  formMessage = '';
  formError = false;

  // Pagination
  page = 1;
  pageSize = 5;
  pageSizeOptions = [5, 10, 20];

  distance: DistanceRequest = {
    distanceId: undefined,
    address: '',
    city: '',
    country: ''
  };

  constructor(
    private distanceService: DistanceService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadUserFromLocalStorage();
    this.loadDistances();
  }

  private loadUserFromLocalStorage(): void {
    this.userName = localStorage.getItem('userName') || 'User';
    const rolesStr = localStorage.getItem('roles');
    this.roles = rolesStr ? JSON.parse(rolesStr) : [];
  }

  loadDistances(): void {
    this.loadingDistances = true;
    this.distanceService.getAll().subscribe({
      next: res => {
        this.allDistances = res || [];
        this.originalDistances = [...this.allDistances];
        this.page = 1;
        this.applyPagination();
        this.loadingDistances = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.allDistances = [];
        this.applyPagination();
        this.loadingDistances = false;
        this.cdr.detectChanges();
      }
    });
  }

  searchDistances(): void {
    const text = this.searchText.trim().toLowerCase();
    if (!text) {
      this.allDistances = [...this.originalDistances];
      this.page = 1;
      this.applyPagination();
      return;
    }

    this.allDistances = this.originalDistances.filter(d =>
      d.address.toLowerCase().includes(text) ||
      d.city.toLowerCase().includes(text) ||
      d.country.toLowerCase().includes(text)
    );

    this.page = 1;
    this.applyPagination();
  }

  // Pagination
  get totalPages(): number[] {
    return Array.from(
      { length: Math.ceil(this.allDistances.length / this.pageSize) },
      (_, i) => i + 1
    );
  }

  applyPagination(): void {
    const start = (this.page - 1) * this.pageSize;
    this.distances = this.allDistances.slice(start, start + this.pageSize);
  }

  nextPage(): void {
    if (this.page * this.pageSize < this.allDistances.length) {
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

  // Modal
  openAdd(): void {
    this.isEditMode = false;
    this.formMessage = '';
    this.formError = false;
    this.distance = { distanceId: undefined, address: '', city: '', country: '' };
    this.showAddForm = true;
  }

  editDistance(d: DistanceResponse): void {
    this.isEditMode = true;
    this.formMessage = '';
    this.formError = false;
    this.distance = { ...d }; // include distanceId
    this.showAddForm = true;
  }

  closeForm(): void {
    if (!this.isSubmitting) this.showAddForm = false;
  }

  // Validation
  private validateForm(): boolean {
    if (!this.distance.address?.trim()) return this.setError('Address required');
    if (!this.distance.city?.trim()) return this.setError('City required');
    if (!this.distance.country?.trim()) return this.setError('Country required');
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
    this.distanceService.addOrUpdate(this.distance).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.showAddForm = false;
        this.loadDistances();
        this.cdr.detectChanges();
      },
      error: err => {
        this.isSubmitting = false;
        this.formError = true;
        this.formMessage = err?.error?.message || 'Save failed';
        this.cdr.detectChanges();
      }
    });
  }

  deleteDistance(id: number): void {
    if (!confirm('Delete this distance?')) return;

    this.distanceService.delete(id).subscribe(() => {
      this.loadDistances();
      this.cdr.detectChanges();
    });
  }
}
