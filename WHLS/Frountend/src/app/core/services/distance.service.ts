import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface DistanceRequest {
  distanceId?: number;
  address: string;
  city: string;
  country: string;
}

export interface DistanceResponse {
  distanceId: number;
  address: string;
  city: string;
  country: string;
}

export interface DistanceFilterModel {
  address?: string;
  city?: string;
  country?: string;
}

@Injectable({
  providedIn: 'root'
})
export class DistanceService {
  private baseUrl = 'http://localhost:5001/api/Distance';

  constructor(private http: HttpClient) {}

  getAll(): Observable<DistanceResponse[]> {
    return this.http.get<DistanceResponse[]>(`${this.baseUrl}/all`)
      .pipe(catchError(() => of([])));
  }

  filter(filter: DistanceFilterModel): Observable<DistanceResponse[]> {
    return this.http.post<DistanceResponse[]>(`${this.baseUrl}/filter`, filter)
      .pipe(catchError(() => of([])));
  }

  addOrUpdate(data: DistanceRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/add-or-update`, data)
      .pipe(catchError(err => of(err)));
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`)
      .pipe(catchError(err => of(err)));
  }
}
