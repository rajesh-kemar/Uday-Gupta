import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api';

interface Trip {
  id: number;
  driverId: number;
  vehicleId: number;
  origin: string;
  destination: string;
  startTime: string | null;
  endTime: string | null;
  status: string; // will always be a string (Planned/Active/Completed)
}

@Component({
  selector: 'app-trips',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './trips.component.html',
  styleUrls: ['./trips.component.css']
})
export class TripsComponent implements OnInit {
  form = {
    driverId: '',
    vehicleId: '',
    origin: '',
    destination: '',
    startTime: '',
    endTime: ''
  };

  drivers: any[] = [];
  vehicles: any[] = [];
  trips: Trip[] = [];
  loading = false;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadData();
  }

  private toIsoIfNeeded(v: any): string | null {
    if (!v) return null;
    // If server gives string ISO already, keep it. If Date object, convert.
    return typeof v === 'string' ? v : new Date(v).toISOString();
  }

  /** Load drivers, vehicles, trips and normalize fields */
  loadData(): void {
    this.loading = true;

    this.api.getDrivers().subscribe({
      next: data => (this.drivers = data),
      error: err => console.error('Failed to load drivers:', err)
    });

    // load available vehicles (or all, as you need)
    this.api.getVehicles(true).subscribe({
      next: data => (this.vehicles = data),
      error: err => console.error('Failed to load vehicles:', err)
    });

    this.api.getTrips().subscribe({
      next: data => {
        this.trips = (data || []).map((t: any) => ({
          id: t.id ?? t.Id,
          driverId: t.driverId ?? t.DriverId,
          vehicleId: t.vehicleId ?? t.VehicleId,
          origin: (t.origin ?? t.Origin) ?? '',
          destination: (t.destination ?? t.Destination) ?? '',
          // normalize date/time fields to ISO strings (or null)
          startTime: t.startTime ?? t.StartTime ? (t.startTime ?? t.StartTime) : null,
          endTime: t.endTime ?? t.EndTime ? (t.endTime ?? t.EndTime) : null,
          // convert numeric enum to string name
          status: this.getStatusName(t.status ?? t.Status)
        }));
        this.loading = false;
      },
      error: err => {
        console.error('Failed to load trips:', err);
        this.loading = false;
      }
    });
  }

  /** Add new trip */
  addTrip(): void {
    const { driverId, vehicleId, origin, destination, startTime, endTime } = this.form;

    if (!driverId || !vehicleId || !origin.trim() || !destination.trim() || !startTime || !endTime) {
      alert('Please fill all fields.');
      return;
    }
    const start = new Date(startTime);
    const end = new Date(endTime);
    if (end <= start) { alert('End time must be after start time.'); return; }

    const payload = {
      DriverId: Number(driverId),
      VehicleId: Number(vehicleId),
      Origin: origin.trim(),
      Destination: destination.trim(),
      StartTime: start.toISOString(),
      EndTime: end.toISOString()
    };

    this.api.addTrip(payload).subscribe({
      next: () => {
        alert('Trip added');
        this.form = { driverId: '', vehicleId: '', origin: '', destination: '', startTime: '', endTime: '' };
        this.loadData();
      },
      error: err => {
        console.error('Failed to add trip:', err);
        alert('Failed to add trip (check console).');
      }
    });
  }

  /** Toggle trip status: Planned -> Active -> Completed */
  toggleTripStatus(trip: Trip): void {
    if (trip.status === 'Planned') {
      if (!confirm(`Start trip #${trip.id}?`)) return;
      this.api.startTrip(trip.id).subscribe({
        next: () => { trip.status = 'Active'; this.loadData(); },
        error: err => { console.error('Failed to start trip:', err); alert('Failed to start trip'); }
      });
    } else if (trip.status === 'Active') {
      if (!confirm(`Complete trip #${trip.id}?`)) return;
      this.api.completeTrip(trip.id).subscribe({
        next: () => { trip.status = 'Completed'; this.loadData(); },
        error: err => { console.error('Failed to complete trip:', err); alert('Failed to complete trip'); }
      });
    }
  }

  /** Convert server enum or string to readable status name */
  getStatusName(statusValue: any): string {
    // If backend already returns a string, use it.
    if (typeof statusValue === 'string') return statusValue;

    // numeric enum -> map to readable name
    switch (statusValue) {
      case 0: return 'Planned';
      case 1: return 'Active';
      case 2: return 'Completed';
      default: return 'Planned'; // fallback
    }
  }

  // helpers to display driver/vehicle
  getDriverName(id: number): string {
    return this.drivers.find(d => d.id === id)?.name || 'Unknown';
  }

  getVehicleNumber(id: number): string {
    return this.vehicles.find(v => v.id === id)?.vehicleNumber || 'Unknown';
  }
}
