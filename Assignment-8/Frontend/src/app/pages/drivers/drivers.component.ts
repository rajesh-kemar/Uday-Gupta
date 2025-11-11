import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { DriverService } from '../../services/driver.service';
import { TripService } from '../../services/trip.service';

@Component({
  selector: 'app-drivers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './drivers.component.html',
  styleUrls: ['./drivers.component.css']
})
export class DriversComponent implements OnInit {
  drivers: any[] = [];
  trips: any[] = [];
  selectedDriver: any = {
    id: 0,
    name: '',
    licenseNumber: '',
    phone: '',
    experience: 0,
    isAvailable: true
  };
  isEditing = false;
  errorMessage = '';

  constructor(
    private driverService: DriverService,
    private tripService: TripService
  ) {}

  ngOnInit() {
    this.loadDriversAndTrips();
  }

  //  Load drivers and trips
  loadDriversAndTrips() {
    this.driverService.getDrivers().subscribe({
      next: (driversRes) => {
        this.drivers = driversRes;
        this.tripService.getTrips().subscribe({
          next: (tripsRes) => {
            this.trips = tripsRes;
            this.updateDriverAvailabilityBasedOnTrips();
          },
          error: (err) => {
            console.error('Error loading trips:', err);
            this.errorMessage = 'Failed to load trips.';
          }
        });
      },
      error: (err) => {
        console.error('Error loading drivers:', err);
        this.errorMessage = 'Failed to load drivers.';
      }
    });
  }

  //  Mark drivers unavailable if on active trip
  updateDriverAvailabilityBasedOnTrips() {
    const busyDriverIds = this.trips
      .filter(trip => trip.status === 'Active')
      .map(trip => trip.driverId);

    this.drivers.forEach(driver => {
      driver.isAvailable = !busyDriverIds.includes(driver.id);
    });
  }

  //  Phone number validation helper
  isPhoneInvalid(phone: string): boolean {
    if (!phone) return false;
    return !/^[0-9]{10}$/.test(phone);
  }

  //  Save or Update driver
  saveDriver(form: NgForm) {
    if (form.invalid || this.isPhoneInvalid(this.selectedDriver.phone)) return;

    if (this.isEditing) {
      const updatedDriver = { ...this.selectedDriver };
      this.driverService.updateDriver(updatedDriver).subscribe({
        next: () => {
          alert('Driver updated successfully!');
          this.resetForm(form);
          this.loadDriversAndTrips();
        },
        error: (err) => {
          console.error('Error updating driver:', err);
          this.errorMessage = 'Error updating driver.';
        }
      });
    } else {
      const newDriver = { ...this.selectedDriver };
      this.driverService.addDriver(newDriver).subscribe({
        next: () => {
          alert(' Driver added successfully!');
          this.resetForm(form);
          this.loadDriversAndTrips();
        },
        error: (err) => {
          console.error('Error adding driver:', err);
          this.errorMessage = 'Error adding driver.';
        }
      });
    }
  }

  editDriver(driver: any) {
    this.selectedDriver = { ...driver };
    this.isEditing = true;
  }

  deleteDriver(id: number) {
    if (confirm('Are you sure you want to delete this driver?')) {
      this.driverService.deleteDriver(id).subscribe({
        next: () => {
          alert('🗑️ Driver deleted successfully!');
          this.loadDriversAndTrips();
        },
        error: (err) => {
          console.error('Error deleting driver:', err);
          this.errorMessage = 'Error deleting driver.';
        }
      });
    }
  }

  resetForm(form: NgForm) {
    form.resetForm();
    this.isEditing = false;
    this.selectedDriver = {
      id: 0,
      name: '',
      licenseNumber: '',
      phone: '',
      experience: 0,
      isAvailable: true
    };
    this.errorMessage = '';
  }
}
