import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'https://localhost:7280/api'; // ✅ Backend URL

  constructor(private http: HttpClient) {}

  // ---------------- DRIVERS ----------------
  getDrivers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/drivers`);
  }

  addDriver(driver: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/drivers`, driver);
  }

  // ---------------- VEHICLES ----------------
  getVehicles(available?: boolean): Observable<any[]> {
    let url = `${this.baseUrl}/vehicles`;
    if (available !== undefined) {
      url += `?available=${available}`;
    }
    return this.http.get<any[]>(url);
  }

  addVehicle(vehicle: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/vehicles`, vehicle);
  }

  updateVehicle(vehicle: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/vehicles/${vehicle.id}`, vehicle);
  }

  // ---------------- TRIPS ----------------
  getTrips(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/trips`);
  }

  addTrip(trip: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/trips`, trip);
  }

  // ✅ Start Trip — must send {} body
  startTrip(id: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/trips/${id}/start`, {});
  }

  // ✅ Complete Trip — must send {} body
  completeTrip(id: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/trips/${id}/complete`, {});
  }
}
