import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VehicleService {
  private baseUrl = 'http://localhost:5110/api/vehicles';

  constructor(private http: HttpClient) {}

  //  Get all vehicles
  getVehicles(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl);
  }

  //  Get only available vehicles
  getAvailableVehicles(): Observable<any[]> {
    //  fixed URL (was api/vehicles/vehicles/available)
    return this.http.get<any[]>(`${this.baseUrl}/available`);
  }

  //  Add new vehicle
  addVehicle(vehicle: any): Observable<any> {
    return this.http.post(this.baseUrl, vehicle);
  }

  //  Update existing vehicle
  updateVehicle(vehicle: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/${vehicle.id}`, vehicle);
  }

  //  Delete vehicle by ID
  deleteVehicle(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
