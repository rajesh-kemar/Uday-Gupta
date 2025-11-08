import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TripService } from '../../services/trip.service';
import { Trip } from '../../models/trip.model';

@Component({
  selector: 'app-trip-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './trip-list.component.html',
  styleUrls: ['./trip-list.component.css'] //  Link your CSS file here
})
export class TripListComponent implements OnInit {
  trips: Trip[] = [];
  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(private tripService: TripService) {}

  ngOnInit(): void {
    this.loadTrips();
  }

  /** 🔹 Load all trips from backend */
  loadTrips(): void {
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.tripService.getTrips().subscribe({
      next: (data: Trip[]) => {
        this.trips = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('❌ Error loading trips:', err);
        this.errorMessage = 'Failed to load trips. Please try again later.';
        this.loading = false;
      }
    });
  }

  /** 🔹 Mark a trip as completed */
  markCompleted(trip: Trip): void {
    if (!trip.id || trip.isCompleted) return;

    const updatedTrip: Trip = { ...trip, isCompleted: true };

    this.tripService.markTripCompleted(trip.id).subscribe({
      next: () => {
        const index = this.trips.findIndex(t => t.id === trip.id);
        if (index !== -1) {
          this.trips[index] = updatedTrip;
        }
        this.successMessage = '✅ Trip marked as completed successfully!';
      },
      error: (err) => {
        console.error('❌ Error marking trip as completed:', err);
        this.errorMessage = 'Failed to mark trip as completed. Please try again later.';
      }
    });
  }

  /** Optional: trackBy for better performance */
  trackByTripId(index: number, trip: Trip): number {
    return trip.id ?? index;
  }
}
