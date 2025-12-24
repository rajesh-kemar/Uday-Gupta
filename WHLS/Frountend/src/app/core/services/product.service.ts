import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface ProductResponse {
  productId: number;
  name: string;
  description?: string;
  weight: number;
}

export interface ProductRequest {
  productId?: number;
  name: string;
  description?: string;
  weight: number;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = 'http://localhost:5001/api/product';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ProductResponse[]> {
    return this.http.get<ProductResponse[]>(this.apiUrl).pipe(catchError(() => of([])));
  }

  addOrUpdate(data: ProductRequest) {
    return this.http.post(`${this.apiUrl}/add-or-update`, data);
  }

  delete(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  filter(filter: any) {
    return this.http.post<ProductResponse[]>(`${this.apiUrl}/filter`, filter);
  }

  // ✅ Add this method for products by warehouse
  getByWarehouse(warehouseId: number): Observable<ProductResponse[]> {
    return this.http.get<ProductResponse[]>(`${this.apiUrl}/by-warehouse/${warehouseId}`)
      .pipe(catchError(() => of([])));
  }
}
