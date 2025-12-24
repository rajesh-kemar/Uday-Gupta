import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  email = '';
  password = '';
  errorMessage = '';
  loading = false;
  showPassword = false;

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    this.loading = true;
    this.errorMessage = '';

    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: (res: any) => {
        this.loading = false;

        // ✅ SAVE EVERYTHING
        localStorage.setItem('token', res.token);
        localStorage.setItem('userName', res.userName || res.email);

        // IMPORTANT: roles MUST be an array
        if (Array.isArray(res.roles)) {
          localStorage.setItem('roles', JSON.stringify(res.roles));
        } else if (res.role) {
          localStorage.setItem('roles', JSON.stringify([res.role]));
        } else {
          localStorage.setItem('roles', JSON.stringify([]));
        }

        console.log('LOGIN DATA:', {
          token: res.token,
          userName: res.userName,
          roles: res.roles
        });

        this.router.navigate(['/dashboard']);
      },
      error: (err: any) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Invalid email or password';
      }
    });
  }
}
