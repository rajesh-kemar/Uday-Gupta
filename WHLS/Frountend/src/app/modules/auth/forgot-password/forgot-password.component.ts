import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {

  email = '';
  newPassword = '';
  confirmPassword = '';

  showPassword = false;
  isLoading = false;

  message = '';
  error = '';

  constructor(
    private http: HttpClient,
    private router: Router,
    private cdr: ChangeDetectorRef // ✅ FIX
  ) {}

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  submit(): void {
    this.error = '';
    this.message = '';

    if (this.newPassword !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    this.isLoading = true;
    this.cdr.detectChanges();

    this.http.post('http://localhost:5001/api/auth/forgot-password', {
      email: this.email,
      newPassword: this.newPassword,
      confirmPassword: this.confirmPassword
    }).subscribe({
  next: () => {
    alert('Password reset successful. Please login with new password.');
    this.router.navigate(['/login']);
  },
  error: err => {
    this.isLoading = false;
    this.error = err.error?.message || 'Something went wrong';
    this.cdr.detectChanges();
  }
});

  }
}
