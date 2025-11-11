import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username = '';
  password = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  onLogin(form: NgForm) {
    if (form.invalid) {
      this.errorMessage = 'Please fill out all required fields.';
      return;
    }

    const credentials = { username: this.username, password: this.password };

    this.authService.login(credentials).subscribe({
      next: (response: any) => {
        const role = this.authService.getUserRole();

        if (role === 'Driver') {
          this.router.navigate(['/driver-dashboard']);
        } else if (role === 'Dispatcher') {
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage = 'Unknown user role.';
        }
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Login failed. Please check your credentials.';
      }
    });
  }
}
