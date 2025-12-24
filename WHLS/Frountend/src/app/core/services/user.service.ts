import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserResponse {
  userId: number;
  username: string;
  email: string;
  role: string;
}

export interface UserRequest {
  userId?: number;
  username: string;
  email: string;
  password?: string;
  role: string;
  roleIds: number[];
}

@Injectable({ providedIn: 'root' })
export class UserService {

  private apiUrl = 'http://localhost:5001/api/user';

  constructor(private http: HttpClient) {}

  getUsers(): Observable<UserResponse[]> {
    return this.http.get<UserResponse[]>(`${this.apiUrl}/all`);
  }

  addOrUpdateUser(payload: UserRequest) {
    return this.http.post(`${this.apiUrl}/add-or-update`, payload);
  }

  deleteUser(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  // ✅ exact backend filter model
  filterUsers(filter: {
    username?: string;
    email?: string;
    role?: string;
  }) {
    return this.http.post<UserResponse[]>(`${this.apiUrl}/filter`, filter);
  }
}
