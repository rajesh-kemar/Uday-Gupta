import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private baseUrl = 'http://localhost:5001/api';

  constructor(private http: HttpClient) {}

  /** Generic method to get count from any endpoint */
  private getCount(url: string): Observable<number> {
    return this.http.get<any>(url).pipe(
      map(res => {
        // Handle different backend response shapes
        if (!res) return 0;
        if (Array.isArray(res)) return res.length;
        if (res.data && Array.isArray(res.data)) return res.data.length;
        if (res.result && Array.isArray(res.result)) return res.result.length;
        return 0;
      }),
      catchError(() => of(0)) // return 0 if API fails
    );
  }

  // ✅ Count methods for dashboard
  getProductsCount(): Observable<number> {
    return this.getCount(`${this.baseUrl}/Product`);
  }

  getVehiclesCount(): Observable<number> {
    return this.getCount(`${this.baseUrl}/Vehicle/all`);
  }

  getWarehousesCount(): Observable<number> {
    return this.getCount(`${this.baseUrl}/Warehouse/all`);
  }

  getInventoriesCount(): Observable<number> {
    return this.getCount(`${this.baseUrl}/Inventory/all`);
  }

  getShipmentsCount(): Observable<number> {
    return this.getCount(`${this.baseUrl}/Shipment/all`);
  }

  getDistancesCount(): Observable<number> {
    return this.getCount(`${this.baseUrl}/Distance/all`);
  }

  getUsersCount(): Observable<number> {
    return this.getCount(`${this.baseUrl}/User/all`);
  }
}
