import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface WarehouseRequest {
  warehouseId?: number;
  name: string;
  location: string;
  capacity: number;
}

export interface WarehouseResponse {
  warehouseId: number;
  name: string;
  location: string;
  capacity: number;
}

@Injectable({ providedIn: 'root' })
export class WarehouseService {
  private baseUrl = 'http://localhost:5001/api/Warehouse';

  constructor(private http: HttpClient) {}

  getAll(): Observable<WarehouseResponse[]> {
    return this.http.get<WarehouseResponse[]>(`${this.baseUrl}/all`).pipe(catchError(() => of([])));
  }

  filter(filter: any): Observable<WarehouseResponse[]> {
    return this.http.post<WarehouseResponse[]>(`${this.baseUrl}/filter`, filter).pipe(catchError(() => of([])));
  }

  addOrUpdate(data: WarehouseRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/add-or-update`, data).pipe(catchError(err => of(err)));
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`).pipe(catchError(err => of(err)));
  }
}
