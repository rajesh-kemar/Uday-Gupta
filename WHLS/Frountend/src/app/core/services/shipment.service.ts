import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ShipmentRequest {
  shipmentId?: number;
  shipmentNumber: string;
  shipmentDate: string;
  destinationId: number;
  vehicleId: number;
  status?: string;
}

export interface ShipmentResponse {
  shipmentId: number;
  shipmentNumber: string;
  shipmentDate: string;
  destinationId: number;
  destinationAddress: string;
  vehicleId: number;
  vehicleNumber: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class ShipmentService {

  private baseUrl = 'http://localhost:5001/api/Shipment';

  constructor(private http: HttpClient) {}

  private authHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  getAll(): Observable<ShipmentResponse[]> {
    return this.http.get<ShipmentResponse[]>(`${this.baseUrl}/all`, { headers: this.authHeaders() });
  }

  addOrUpdate(data: ShipmentRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/add-or-update`, data, { headers: this.authHeaders() });
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`, { headers: this.authHeaders() });
  }
}
