import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  drivers: any[] = [];
  vehicles: any[] = [];
  trips: any[] = [];

  loading = true;
  error: string = '';
  expanded: string | null = null;

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.loadData();
    setInterval(() => this.loadData(), 10000); // refresh every 10s
  }

  loadData() {
    this.loading = true;
    this.error = '';

    this.api.getDrivers().subscribe({
      next: data => (this.drivers = data),
      error: err => console.error('Error loading drivers:', err)
    });

    this.api.getVehicles().subscribe({
      next: data => (this.vehicles = data),
      error: err => console.error('Error loading vehicles:', err)
    });

    this.api.getTrips().subscribe({
      next: data => {
        this.trips = (data || []).map((t: any) => {
          const raw = t.status ?? t.Status;
          let s = '';

          if (typeof raw === 'number') {
            s = raw === 1 ? 'active' : raw === 2 ? 'completed' : 'planned';
          } else if (typeof raw === 'string') {
            s = raw.trim().toLowerCase();
            if (!s && !isNaN(Number(raw))) {
              const n = Number(raw);
              s = n === 1 ? 'active' : n === 2 ? 'completed' : 'planned';
            }
          } else {
            s = '';
          }

          return {
            ...t,
            statusOriginal: raw,
            statusNormalized: s
          };
        });
        this.loading = false;
        console.log('trips loaded:', this.trips);
      },
      error: err => {
        console.error('Error loading trips:', err);
        this.error = 'Error loading trips';
        this.loading = false;
      }
    });
  }

  // metrics
  get totalDrivers() { return this.drivers.length; }
  get totalVehicles() { return this.vehicles.length; }
  get availableVehicles() { return this.availableVehiclesList.length; }
  get activeTrips() { return this.activeTripsList.length; }
  get completedTrips() { return this.completedTripsList.length; }
  get longTrips() { return this.longTripsList.length; }

  get availableVehiclesList() {
    const activeVehicleIds = this.activeTripsList.map(t => t.vehicleId);
    return this.vehicles.filter(v => !activeVehicleIds.includes(v.id));
  }

  get activeTripsList() {
    return this.trips.filter(t => (t.statusNormalized || '').toLowerCase() === 'active');
  }

  get completedTripsList() {
    return this.trips.filter(t => (t.statusNormalized || '').toLowerCase() === 'completed');
  }

  get longTripsList() {
    return this.trips.filter(t => {
      if (!t.startTime || !t.endTime) return false;
      const start = new Date(t.startTime), end = new Date(t.endTime);
      if (isNaN(start.getTime()) || isNaN(end.getTime())) return false;
      return ((end.getTime() - start.getTime()) / (1000 * 60 * 60)) > 8;
    });
  }

  toggleTable(section: string) {
    this.expanded = this.expanded === section ? null : section;
  }

  getKeys(obj: any) {
    return Object.keys(obj || {});
  }
}
