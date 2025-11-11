import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  role: string = 'Driver';
  name: string = '';
  experience: number | null = null;
  username: string = '';
  password: string = '';
  phoneNumber: string = '';
  licenseNumber: string = '';
  showPassword: boolean = false;
  passwordError: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  onRoleChange() {
    if (this.role === 'Dispatcher') {
      alert('Dispatcher accounts can log in directly.');
      this.router.navigate(['/login']);
    }
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  validatePassword() {
    if (this.password.length < 6) {
      this.passwordError = 'Password must be at least 6 characters long.';
    } else {
      this.passwordError = '';
    }
  }

  onRegister(form: NgForm) {
    if (form.invalid || this.passwordError) {
      this.errorMessage = 'Please fill all required fields correctly.';
      return;
    }

    const phoneRegex = /^[0-9]{10}$/;
    if (!phoneRegex.test(this.phoneNumber)) {
      this.errorMessage = 'Phone number must be exactly 10 digits.';
      return;
    }

    const payload = {
      username: this.username,
      password: this.password,
      role: this.role,
      name: this.name,
      licenseNumber: this.licenseNumber,
      phone: this.phoneNumber,
      experience: this.experience,
      isAvailable: true
    };

    console.log('Register payload:', payload);

    this.authService.register(payload).subscribe({
      next: () => {
        alert('Registration successful!');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Registration failed:', err);
        this.errorMessage = err?.error?.message || 'Registration failed.';
      }
    });
  }
}
