import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  @Input() userName: string = '';
  @Input() roles: string[] = [];

  showDropdown = false;
  showLogoutConfirm = false;

  constructor(private router: Router) {}

  // ===== Role helpers =====
  get isAdmin(): boolean {
    return this.roles?.includes('Admin') ?? false;
  }

  get roleText(): string {
    return this.roles?.length ? this.roles.join(', ') : '';
  }

  // ===== Navigation (SAFE) =====
  goTo(path?: string): void {
    this.showDropdown = false;

    if (!path) {
      console.error('Navbar navigation error: path is undefined');
      return;
    }

    this.router.navigateByUrl('/' + path);
  }

  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
  }

  confirmLogout(): void {
    this.showDropdown = false;
    this.showLogoutConfirm = true;
  }

  logout(): void {
    localStorage.clear();
    this.showLogoutConfirm = false;
    this.router.navigateByUrl('/login');
  }

  changePassword(): void {
    this.showDropdown = false;
    this.router.navigateByUrl('/change-password');
  }

  forgotPassword(): void {
    this.showDropdown = false;
    this.router.navigateByUrl('/forgot-password');
  }
}
