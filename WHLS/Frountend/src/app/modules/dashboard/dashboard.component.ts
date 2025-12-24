import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { DashboardService } from '../../core/services/dashboard.service';
import { AuthService } from '../../core/services/auth.service';
import { NavbarComponent } from '../../shared/navbar/navbar.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  userName = 'User';
  roles: string[] = [];

  totalProducts = 0;
  totalVehicles = 0;
  totalWarehouses = 0;
  totalInventories = 0;
  totalShipments = 0;
  totalDistances = 0;
  totalUsers = 0;

  pageReady = false;

  constructor(
    private dashboardService: DashboardService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef // ✅ FIX
  ) {}

  ngOnInit(): void {
    this.userName = this.authService.getUserName();
    this.roles = this.authService.getRoles();
    this.loadDashboardCounts();
  }

  private loadDashboardCounts(): void {
    forkJoin({
      products: this.dashboardService.getProductsCount(),
      vehicles: this.dashboardService.getVehiclesCount(),
      warehouses: this.dashboardService.getWarehousesCount(),
      inventories: this.dashboardService.getInventoriesCount(),
      shipments: this.dashboardService.getShipmentsCount(),
      distances: this.dashboardService.getDistancesCount(),
      users: this.dashboardService.getUsersCount()
    }).subscribe({
      next: res => {
        this.totalProducts = res.products;
        this.totalVehicles = res.vehicles;
        this.totalWarehouses = res.warehouses;
        this.totalInventories = res.inventories;
        this.totalShipments = res.shipments;
        this.totalDistances = res.distances;
        this.totalUsers = res.users;

        this.pageReady = true;
        this.cdr.detectChanges(); // 🔥 IMPORTANT
      },
      error: () => {
        this.pageReady = true;
        this.cdr.detectChanges();
      }
    });
  }
}
