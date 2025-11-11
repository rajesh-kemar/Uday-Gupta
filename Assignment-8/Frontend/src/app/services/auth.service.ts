import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'http://localhost:5110/api/Auth'; 
  private jwtHelper = new JwtHelperService();

  constructor(private http: HttpClient, private router: Router) {}

  //  Login user
  login(credentials: { username: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, credentials).pipe(
      tap((res: any) => {
       if (res.token) {
  this.saveToken(res.token);
  this.saveRole(res.role);
  if (res.driverId) {
    localStorage.setItem('driverId', res.driverId.toString());
  }
}
      })
    );
  }

  //  Register user
  register(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, payload);
  }

  //  Save token & role
  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  saveRole(role: string): void {
    localStorage.setItem('role', role);
  }

  //  Getters
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  //  Check if user is logged in
  isLoggedIn(): boolean {
    const token = this.getToken();
    return token ? !this.jwtHelper.isTokenExpired(token) : false;
  }

  //  Logout
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    this.router.navigate(['/login']);
  }

  // Decode Token (Optional helper)
  private decodeToken(): any | null {
    const token = this.getToken();
    try {
      return token ? this.jwtHelper.decodeToken(token) : null;
    } catch {
      return null;
    }
  }

  getUsername(): string | null {
    const decoded = this.decodeToken();
    return decoded?.['unique_name'] || decoded?.['name'] || null;
  }

  getUserRole(): string | null {
    // Prefer stored role (backend is always correct)
    return this.getRole();
  }

  getUser() {
  const userData = localStorage.getItem('user');
  if (userData) {
    try {
      return JSON.parse(userData);
    } catch (e) {
      console.error('Invalid user data in localStorage', e);
      return null;
    }
  }
  return null;
}

getDriverId(): string | null {
  return localStorage.getItem('driverId');
}


}
