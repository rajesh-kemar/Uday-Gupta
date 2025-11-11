import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TripService {
  private baseUrl = 'http://localhost:5110/api/Trips';

  constructor(private http: HttpClient) {}

  private getHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      })
    };
  }

  getTrips(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl, this.getHeaders());
  }

  getTrip(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${id}`, this.getHeaders());
  }

  addTrip(trip: any): Observable<any> {
    return this.http.post<any>(this.baseUrl, trip, this.getHeaders());
  }

  updateTrip(id: number, trip: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${id}`, trip, this.getHeaders());
  }

  deleteTrip(id: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/${id}`, this.getHeaders());
  }

  //  NEW — mark trip completed
  markTripCompleted(id: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}/complete`, {}, this.getHeaders());
  }

  getDriverSummary(driverId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/summary/${driverId}`, this.getHeaders());
  }
}
