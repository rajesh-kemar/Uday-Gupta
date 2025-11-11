import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DriverService } from '../../services/driver.service';
import { VehicleService } from '../../services/vehicle.service';
import { TripService } from '../../services/trip.service';
import { AuthService } from '../../services/auth.service';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  totalDrivers = 0;
  totalVehicles = 0;
  totalTrips = 0;
  activeTrips = 0;
  completedTrips = 0;

  role: string | null = null;

  constructor(
    private driverService: DriverService,
    private vehicleService: VehicleService,
    private tripService: TripService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.role = this.authService.getUserRole();

    // 🚫 Prevent driver from viewing dispatcher dashboard
    if (this.role === 'Driver') {
      alert('Access denied. Redirecting to Driver Dashboard.');
      this.router.navigate(['/driver-dashboard']);
      return;
    }

    //  Load dashboard data only for dispatchers
    this.loadDashboardData();
  }

  loadDashboardData() {
  this.driverService.getDrivers().subscribe((res) => (this.totalDrivers = res.length));
  this.vehicleService.getVehicles().subscribe((res) => (this.totalVehicles = res.length));
  this.tripService.getTrips().subscribe((res) => {
    this.totalTrips = res.length;
    this.activeTrips = res.filter((t: any) => t.status === 'Active').length;
    this.completedTrips = res.filter((t: any) => t.status === 'Completed').length; 
  });
}


  logout() {
    this.authService.logout();
  }
}
