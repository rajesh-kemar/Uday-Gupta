import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TripService } from '../../services/trip.service';
import { DriverService } from '../../services/driver.service';
import { VehicleService } from '../../services/vehicle.service';

@Component({
  selector: 'app-trips',
  standalone: true,
  templateUrl: './trips.component.html',
  styleUrls: ['./trips.component.css'],
  imports: [CommonModule, FormsModule],
})
export class TripsComponent implements OnInit {
  trips: any[] = [];
  drivers: any[] = [];
  vehicles: any[] = [];

  newTrip: any = {
    id: null,
    driverId: null,
    vehicleId: null,
    origin: '',
    destination: '',
    startTime: '',
    status: 'Active',
  };

  editMode = false;
  loading = false;
  errorMessage = '';

  constructor(
    private tripService: TripService,
    private driverService: DriverService,
    private vehicleService: VehicleService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.refreshData();
  }

  //  Load trips, drivers, and vehicles
  refreshData() {
    this.loading = true;
    this.tripService.getTrips().subscribe({
      next: (res) => {
        this.trips = res.map((t) => ({
          ...t,
          startTime: t.startTime ? new Date(t.startTime) : null,
          endTime: t.endTime ? new Date(t.endTime) : null,
        }));
        this.loadAvailableDrivers();
        this.loadAvailableVehicles();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading trips:', err);
        this.errorMessage = 'Failed to load trips.';
        this.loading = false;
      },
      complete: () => (this.loading = false),
    });
  }

  loadAvailableDrivers() {
    this.driverService.getDrivers().subscribe({
      next: (driversRes) => {
        const busyDriverIds = this.trips
          .filter((trip) => trip.status === 'Active')
          .map((trip) => trip.driverId);
        this.drivers = driversRes.map((driver) => ({
          ...driver,
          isAvailable: !busyDriverIds.includes(driver.id),
        }));
      },
      error: (err) => console.error('Error loading drivers:', err),
    });
  }

  loadAvailableVehicles() {
    this.vehicleService.getVehicles().subscribe({
      next: (vehiclesRes) => {
        const busyVehicleIds = this.trips
          .filter((trip) => trip.status === 'Active')
          .map((trip) => trip.vehicleId);
        this.vehicles = vehiclesRes.map((vehicle) => ({
          ...vehicle,
          isAvailable: !busyVehicleIds.includes(vehicle.id),
        }));
      },
      error: (err) => console.error('Error loading vehicles:', err),
    });
  }

  // Convert JS date to datetime-local format
  formatForDateTimeLocal(date: Date | string): string {
    if (!date) return '';
    const d = new Date(date);
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(
      d.getHours()
    )}:${pad(d.getMinutes())}`;
  }

  //  Create or Update Trip
  saveTrip() {
    this.errorMessage = '';

    if (!this.newTrip.driverId || !this.newTrip.vehicleId) {
      this.errorMessage = 'Please select both Driver and Vehicle.';
      return;
    }

    if (!this.newTrip.origin || !this.newTrip.destination) {
      this.errorMessage = 'Please fill both Origin and Destination.';
      return;
    }

  const payload = {
  driverId: Number(this.newTrip.driverId),
  vehicleId: Number(this.newTrip.vehicleId),
  origin: this.newTrip.origin,
  destination: this.newTrip.destination,
  startTime: new Date(this.newTrip.startTime).toISOString(),
  endTime: this.newTrip.endTime ? new Date(this.newTrip.endTime).toISOString() : null,
  status: this.newTrip.status,
};

    this.loading = true;

    if (this.editMode && this.newTrip.id) {
      this.tripService.updateTrip(this.newTrip.id, payload).subscribe({
        next: () => {
          this.refreshData();
          this.resetForm();
        },
        error: (err) => {
          console.error('Error updating trip:', err);
          this.errorMessage = err.error?.message || 'Failed to update trip.';
          this.loading = false;
        },
        complete: () => (this.loading = false),
      });
    } else {
      this.tripService.addTrip(payload).subscribe({
        next: () => {
          this.refreshData();
          this.resetForm();
        },
        error: (err) => {
          console.error('Error adding trip:', err);
          this.errorMessage = err.error?.message || 'Failed to add trip.';
          this.loading = false;
        },
        complete: () => (this.loading = false),
      });
    }
  }

  //  Edit Trip
editTrip(trip: any) {
  this.newTrip = {
    id: trip.id,
    driverId: trip.driverId,
    vehicleId: trip.vehicleId,
    origin: trip.origin,
    destination: trip.destination,
    startTime: this.formatForDateTimeLocal(trip.startTime),
    endTime: trip.endTime ? this.formatForDateTimeLocal(trip.endTime) : '',
    status: trip.status,
  };
  this.editMode = true;
}


  //  Delete Trip
  deleteTrip(id: number) {
    if (confirm('Are you sure you want to delete this trip?')) {
      this.tripService.deleteTrip(id).subscribe({
        next: () => this.refreshData(),
        error: (err) => console.error('Error deleting trip:', err),
      });
    }
  }

  //  Reset Form
  resetForm() {
    this.newTrip = {
      id: null,
      driverId: null,
      vehicleId: null,
      origin: '',
      destination: '',
      startTime: this.formatForDateTimeLocal(new Date()),
      status: 'Active',
    };
    this.editMode = false;
    this.errorMessage = '';
  }

  //  NEW — Mark Trip as Completed
 markTripCompleted(trip: any) {
  if (!confirm('Are you sure you want to mark this trip as completed?')) return;

  const payload = {
    driverId: trip.driverId,
    vehicleId: trip.vehicleId,
    origin: trip.origin,
    destination: trip.destination,
    startTime: new Date(trip.startTime).toISOString(), 
    endTime: new Date().toISOString(),                
    status: 'Completed'
  };

  this.tripService.updateTrip(trip.id, payload).subscribe({
    next: () => {
      alert('Trip marked as completed successfully!');
      this.refreshData();
    },
    error: (err) => {
      console.error('Failed to mark trip completed:', err);
      this.errorMessage = err.error?.message || 'Unable to complete trip.';
    },
  });
}


}
