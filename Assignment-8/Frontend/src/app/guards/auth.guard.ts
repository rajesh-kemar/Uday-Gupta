import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    // 1) check login
    if (!this.authService.isLoggedIn()) {
      alert('Please log in to continue');
      this.router.navigate(['/login']);
      return false;
    }

    // 2) Optional role check (route.data.role)
    const expectedRole = route.data?.['role'] as string | undefined;
    if (!expectedRole) {
      // no role requirement, allow
      return true;
    }

    const userRole = this.authService.getUserRole();
    if (userRole === expectedRole) {
      return true; // allowed
    }

    // If user logged in but wrong role -> redirect to appropriate landing
    if (userRole === 'Driver') {
      this.router.navigate(['/driver-dashboard']);
    } else if (userRole === 'Dispatcher') {
      this.router.navigate(['/dashboard']);
    } else {
      // fallback
      this.router.navigate(['/login']);
    }

    return false;
  }
}
