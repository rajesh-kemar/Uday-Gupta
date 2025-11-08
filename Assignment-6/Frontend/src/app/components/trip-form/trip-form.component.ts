import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TripService } from '../../services/trip.service';

@Component({
  selector: 'app-trip-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './trip-form.component.html',
  styleUrls: ['./trip-form.component.css']
})
export class TripFormComponent implements OnInit {
  tripForm!: FormGroup;
  vehicles: any[] = [];
  trips: any[] = [];

  constructor(private fb: FormBuilder, private tripService: TripService) {}

  ngOnInit(): void {
    this.initForm();
    this.loadVehicles();
    this.loadTrips();
  }

  /** Initialize form */
  private initForm(): void {
    this.tripForm = this.fb.group({
      driverId: [null, Validators.required],
      vehicleId: [null, Validators.required],
      source: ['', Validators.required],
      destination: ['', Validators.required],
      startingDate: ['', Validators.required],
      endingDate: ['', Validators.required],
      isCompleted: [false]
    });
  }

  /** Load available vehicles */
  private loadVehicles(): void {
    this.tripService.getVehicles().subscribe({
      next: (data: any[]) => {
        this.vehicles = data;
        console.log('🚗 Vehicles loaded:', data);
      },
      error: (err: any) => console.error('❌ Error fetching vehicles:', err)
    });
  }

  /** Load trips */
  private loadTrips(): void {
    this.tripService.getTrips().subscribe({
      next: (data: any[]) => {
        this.trips = data;
        console.log('🧾 Trips loaded:', data);
      },
      error: (err: any) => console.error('❌ Error fetching trips:', err)
    });
  }

  /** Submit trip */
  onSubmit(): void {
    if (this.tripForm.invalid) {
      this.tripForm.markAllAsTouched();
      return;
    }

    const tripData = this.tripForm.value;
    this.tripService.createTrip(tripData).subscribe({
      next: () => {
        alert('✅ Trip created successfully!');
        this.tripForm.reset();
        this.loadTrips();
      },
      error: (err: any) => {
        console.error('❌ Error creating trip:', err);
        alert('Error creating trip. Check console for details.');
      }
    });
  }

  /** Mark as completed */
  markCompleted(trip: any): void {
    if (trip.isCompleted) {
      alert('✅ Trip already completed!');
      return;
    }

    this.tripService.markTripCompleted(trip.id).subscribe({
      next: () => {
        alert(`Trip ${trip.id} marked as completed!`);
        this.loadTrips();
      },
      error: (err: any) => {
        console.error('❌ Error marking trip completed:', err);
        alert('Failed to update trip status.');
      }
    });
  }
}
