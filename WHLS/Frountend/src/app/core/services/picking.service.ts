import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PickingRequest {
  shipmentId: number;
  productId: number;
  warehouseId: number;
  pickedQuantity: number;
}

export interface PickingResponse {
  pickingId: number;
  shipmentId: number;
  productId: number;
  productName: string;
  warehouseId: number;
  warehouseName: string;
  pickedQuantity: number;
}

@Injectable({ providedIn: 'root' })
export class PickingService {

  private baseUrl = 'http://localhost:5001/api/Picking';

  constructor(private http: HttpClient) {}

  private authHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  pick(data: PickingRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/pick`, data, {
      headers: this.authHeaders()
    });
  }

  getByShipment(shipmentId: number): Observable<PickingResponse[]> {
    return this.http.get<PickingResponse[]>(
      `${this.baseUrl}/by-shipment/${shipmentId}`,
      { headers: this.authHeaders() }
    );
  }
}
