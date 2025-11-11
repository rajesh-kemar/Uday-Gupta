import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-driver-dashboard',
  standalone: true,
  imports: [CommonModule, DecimalPipe],
  templateUrl: './driver-dashboard.component.html',
  styleUrls: ['./driver-dashboard.component.css']
})
export class DriverDashboardComponent implements OnInit {
  driver: any = null;
  trips: any[] = [];
  activeTrips = 0;
  completedTrips = 0;
  totalHoursDriven = 0;

  constructor(private http: HttpClient, private auth: AuthService) {}

  ngOnInit() {
    const driverId = this.auth.getDriverId();

    if (!driverId) {
      console.error('No driverId found in localStorage');
      return;
    }

    //  Load driver info
    this.http.get<any>(`http://localhost:5110/api/Drivers/${driverId}`).subscribe({
      next: (res) => {
        console.log('Driver data:', res);
        // Handle both array and object response
        this.driver = Array.isArray(res) ? res[0] : res;
      },
      error: (err) => console.error('Error loading driver:', err)
    });

    //  Load trips for this driver
    this.http.get<any[]>(`http://localhost:5110/api/Trips/driver/${driverId}`).subscribe({
      next: (res) => {
        //console.log('Trip data:', res);
        this.trips = res.map(t => ({
          ...t,
          hours: t.endTime
            ? ((new Date(t.endTime).getTime() - new Date(t.startTime).getTime()) / (1000 * 60 * 60))
            : null
        }));

        //  Calculate counts and totals
        this.activeTrips = this.trips.filter(t => t.status === 'Active').length;
        this.completedTrips = this.trips.filter(t => t.status === 'Completed').length;
        this.totalHoursDriven = this.trips
          .filter(t => t.status === 'Completed' && t.hours)
          .reduce((sum, t) => sum + t.hours, 0);
      },
      error: (err) => console.error('Error loading trips:', err)
    });
  }
}
