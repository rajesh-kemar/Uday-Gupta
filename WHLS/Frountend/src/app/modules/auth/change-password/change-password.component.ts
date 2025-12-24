import { Component, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {

  oldPassword = '';
  newPassword = '';
  confirmPassword = '';

  showPassword = false;
  isLoading = false;
  error = '';
  success = '';

  constructor(
    private http: HttpClient,
    private router: Router,
    private cdr: ChangeDetectorRef // ✅ FIX
  ) {}

  changePassword(): void {
    this.error = '';
    this.success = '';

    if (!this.oldPassword || !this.newPassword || !this.confirmPassword) {
      this.error = 'All fields are required';
      return;
    }

    if (this.newPassword.length < 6) {
      this.error = 'New password must be at least 6 characters';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    const token = localStorage.getItem('token');
    if (!token) {
      this.error = 'Session expired. Please login again.';
      return;
    }

    this.isLoading = true;
    this.cdr.detectChanges();

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    this.http.post(
      'http://localhost:5001/api/auth/change-password',
      {
        oldPassword: this.oldPassword,
        newPassword: this.newPassword,
        confirmPassword: this.confirmPassword
      },
      { headers }
    )
    .pipe(finalize(() => {
      this.isLoading = false;
      this.cdr.detectChanges();
    }))
    .subscribe({
  next: () => {
    alert('Password updated successfully. Please login again.');

    localStorage.clear();
    this.router.navigate(['/login']);
  },
  error: err => {
    this.error = err.error?.message || 'Old password is incorrect';
    this.cdr.detectChanges();
  }
});

  }
}
