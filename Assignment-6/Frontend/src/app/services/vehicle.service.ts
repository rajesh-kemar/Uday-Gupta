import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Vehicle } from '../models/vehicle.model';

@Injectable({ providedIn: 'root' })
export class VehicleService {
  private apiUrl = 'https://localhost:7280/vehicles';

  constructor(private http: HttpClient) {}

  getAvailableVehicles(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(`${this.apiUrl}/available`);
  }
}
