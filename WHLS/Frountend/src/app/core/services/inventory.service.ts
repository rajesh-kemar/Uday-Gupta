import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface InventoryRequest {
  inventoryId?: number;
  quantity: number;
  warehouseId: number;
  productId: number;
}

export interface InventoryResponse {
  inventoryId: number;
  quantity: number;
  warehouseId: number;
  productId: number;
  warehouseName: string;
  productName: string;
}

@Injectable({ providedIn: 'root' })
export class InventoryService {

  private baseUrl = 'http://localhost:5001/api/Inventory';

  constructor(private http: HttpClient) {}

  getAll(): Observable<InventoryResponse[]> {
    return this.http
      .get<InventoryResponse[]>(`${this.baseUrl}/all`)
      .pipe(catchError(() => of([])));
  }

  addOrUpdate(data: InventoryRequest): Observable<any> {
    return this.http
      .post(`${this.baseUrl}/add-or-update`, data)
      .pipe(catchError(err => of(err)));
  }

  delete(id: number): Observable<any> {
    return this.http
      .delete(`${this.baseUrl}/${id}`)
      .pipe(catchError(err => of(err)));
  }

  // ✅ SAFE & TYPED
  getByWarehouse(warehouseId: number): Observable<InventoryResponse[]> {
    return this.getAll().pipe(
      map((list: InventoryResponse[]) =>
        list.filter((i: InventoryResponse) => i.warehouseId === warehouseId)
      )
    );
  }

  // ✅ SAFE & TYPED
  getInventory(
    productId: number,
    warehouseId: number
  ): Observable<InventoryResponse | null> {
    return this.getAll().pipe(
      map((list: InventoryResponse[]) =>
        list.find(
          (i: InventoryResponse) =>
            i.productId === productId && i.warehouseId === warehouseId
        ) ?? null
      )
    );
  }
}
