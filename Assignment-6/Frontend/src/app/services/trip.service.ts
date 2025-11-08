import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Trip } from '../models/trip.model';

@Injectable({ providedIn: 'root' })
export class TripService {
  private apiUrl = 'https://localhost:7280/trips';       // Trip endpoints
  private vehicleUrl = 'https://localhost:7280/vehicles'; // Vehicle endpoints

  constructor(private http: HttpClient) {}

  /** 🔹 Fetch all trips */
  getTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(this.apiUrl);
  }

  /** 🔹 Create a new trip */
  createTrip(trip: Trip): Observable<Trip> {
    const tripPayload = {
      driverId: trip.driverId,
      vehicleId: trip.vehicleId,
      source: trip.source,
      destination: trip.destination,
      startingDate: trip.startingDate,
      endingDate: trip.endingDate,
      isCompleted: trip.isCompleted ?? false
    };
    return this.http.post<Trip>(this.apiUrl, tripPayload);
  }

  /** 🔹 Mark a trip as completed */
  markTripCompleted(id: number): Observable<void> {
    console.log(`✅ Marking trip ${id} as completed`);
    return this.http.put<void>(`${this.apiUrl}/${id}`, {}); // PUT with empty body
  }

  /** 🔹 Fetch available vehicles */
  getVehicles(): Observable<any[]> {
    console.log('🚗 Fetching available vehicles');
    return this.http.get<any[]>(this.vehicleUrl);
  }
}
