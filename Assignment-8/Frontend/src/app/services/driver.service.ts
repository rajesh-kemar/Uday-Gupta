import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';


@Injectable({ providedIn: 'root' })
export class DriverService {
  private baseUrl = 'http://localhost:5110/api/drivers';

  constructor(private http: HttpClient) {}

  //  Get all drivers
  getDrivers(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl);
  }

  // Get only available drivers (uses backend endpoint)
  getAvailableDrivers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/available`).pipe(
      // The backend returns { message, count, data }, we extract data array
      map((res: any) => res.data || [])
    );
  }

  //  Get single driver by ID
  getDriverById(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  //  Add new driver
  addDriver(driver: any): Observable<any> {
    return this.http.post(this.baseUrl, driver);
  }

  //  Update driver
  updateDriver(driver: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/${driver.id}`, driver);
  }

  //  Delete driver
  deleteDriver(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
