import { Routes } from '@angular/router';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { DriversComponent } from './pages/drivers/drivers.component';
import { VehiclesComponent } from './pages/vehicles/vehicles.component';
import { TripsComponent } from './pages/trips/trips.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/registraction/register.component';
import { DriverDashboardComponent } from './pages/driver-dashboard/driver-dashboard.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Dispatcher-only pages (Auth + role check via route data)
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard], data: { role: 'Dispatcher' } },
  { path: 'drivers', component: DriversComponent, canActivate: [AuthGuard], data: { role: 'Dispatcher' } },
  { path: 'vehicles', component: VehiclesComponent, canActivate: [AuthGuard], data: { role: 'Dispatcher' } },
  { path: 'trips', component: TripsComponent, canActivate: [AuthGuard], data: { role: 'Dispatcher' } },

  // Driver-only page
  { path: 'driver-dashboard', component: DriverDashboardComponent, canActivate: [AuthGuard], data: { role: 'Driver' } },

  { path: '**', redirectTo: 'login' }
];
