import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { VehicleService } from '../../services/vehicle.service';
import { TripService } from '../../services/trip.service';

@Component({
  selector: 'app-vehicles',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.css']
})
export class VehiclesComponent implements OnInit {
  vehicles: any[] = [];
  trips: any[] = [];
  selectedVehicle: any = { id: 0, registrationNumber: '', model: '', isAvailable: true };
  isEditing = false;
  errorMessage = '';

  constructor(
    private vehicleService: VehicleService,
    private tripService: TripService
  ) {}

  ngOnInit() {
    this.loadVehiclesAndTrips();
  }

  loadVehiclesAndTrips() {
    this.vehicleService.getVehicles().subscribe({
      next: (vehiclesRes) => {
        this.vehicles = vehiclesRes;
        this.tripService.getTrips().subscribe({
          next: (tripsRes) => {
            this.trips = tripsRes;
            this.updateVehicleAvailabilityBasedOnTrips();
          },
          error: (err) => {
            console.error('Error loading trips:', err);
            this.errorMessage = 'Failed to load trips.';
          }
        });
      },
      error: (err) => {
        console.error('Error loading vehicles:', err);
        this.errorMessage = 'Failed to load vehicles.';
      }
    });
  }

  updateVehicleAvailabilityBasedOnTrips() {
    const busyVehicleIds = this.trips
      .filter(trip => trip.status === 'Active')
      .map(trip => trip.vehicleId);

    this.vehicles.forEach(vehicle => {
      vehicle.isAvailable = !busyVehicleIds.includes(vehicle.id);
    });
  }

  saveVehicle(form: NgForm) {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }

    const vehicleData = {
      id: this.selectedVehicle.id,
      registrationNumber: this.selectedVehicle.registrationNumber.trim(),
      model: this.selectedVehicle.model.trim(),
      isAvailable: this.selectedVehicle.isAvailable
    };

    const action = this.isEditing
      ? this.vehicleService.updateVehicle(vehicleData)
      : this.vehicleService.addVehicle(vehicleData);

    action.subscribe({
      next: () => {
        alert(`✅ Vehicle ${this.isEditing ? 'updated' : 'added'} successfully!`);
        this.resetForm(form);
        this.loadVehiclesAndTrips();
      },
      error: (err) => {
        console.error('Error saving vehicle:', err);
        this.errorMessage = 'Error saving vehicle.';
      }
    });
  }

  editVehicle(vehicle: any) {
    this.selectedVehicle = { ...vehicle };
    this.isEditing = true;
  }

  deleteVehicle(id: number) {
    if (confirm('Are you sure you want to delete this vehicle?')) {
      this.vehicleService.deleteVehicle(id).subscribe({
        next: () => {
          alert('🗑️ Vehicle deleted successfully!');
          this.loadVehiclesAndTrips();
        },
        error: (err) => {
          console.error('Error deleting vehicle:', err);
          this.errorMessage = 'Error deleting vehicle.';
        }
      });
    }
  }

  resetForm(form: NgForm) {
    form.resetForm();
    this.isEditing = false;
    this.selectedVehicle = { id: 0, registrationNumber: '', model: '', isAvailable: true };
    this.errorMessage = '';
  }
}
