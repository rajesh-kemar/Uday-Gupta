import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface VehicleRequest {
  vehicleId?: number;
  vehicleNumber: string;
  type: string;
  capacity: number;
  isActive: boolean;
}

export interface VehicleResponse {
  vehicleId: number;
  vehicleNumber: string;
  type: string;
  capacity: number;
  isActive: boolean;
}

export interface VehicleFilterRequest {
  vehicleNumber?: string;
  type?: string;
  minCapacity?: number;
  maxCapacity?: number;
}

@Injectable({ providedIn: 'root' })
export class VehicleService {

  private baseUrl = 'http://localhost:5001/api/vehicle';

  constructor(private http: HttpClient) {}

  getAll(): Observable<VehicleResponse[]> {
    return this.http.get<VehicleResponse[]>(`${this.baseUrl}/all`)
      .pipe(catchError(() => of([])));
  }

  filter(filter: VehicleFilterRequest): Observable<VehicleResponse[]> {
    return this.http.post<VehicleResponse[]>(`${this.baseUrl}/filter`, filter)
      .pipe(catchError(() => of([])));
  }

  addOrUpdate(vehicle: VehicleRequest): Observable<any> {
    const payload = {
      VehicleId: vehicle.vehicleId ?? null,
      VehicleNumber: vehicle.vehicleNumber,
      Type: vehicle.type,
      Capacity: vehicle.capacity,
      IsActive: vehicle.isActive
    };
    return this.http.post(`${this.baseUrl}/add-or-update`, payload)
      .pipe(catchError(err => of(err)));
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`)
      .pipe(catchError(err => of(err)));
  }
}
