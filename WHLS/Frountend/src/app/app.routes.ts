import { Routes } from '@angular/router';
import { ChangePasswordComponent } from './modules/auth/change-password/change-password.component';
import { DashboardComponent } from './modules/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [

  // DEFAULT
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

  // AUTH
  {
    path: 'login',
    loadComponent: () =>
      import('./modules/auth/login/login.component')
        .then(m => m.LoginComponent)
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./modules/auth/forgot-password/forgot-password.component')
        .then(m => m.ForgotPasswordComponent)
  },
  { path: 'change-password', component: ChangePasswordComponent },

  // DASHBOARD
  {
    path: 'dashboard',
    component: DashboardComponent,
    runGuardsAndResolvers: 'always'
  },

  // USERS
  {
    path: 'users',
    loadComponent: () =>
      import('./modules/user/user.component')
        .then(m => m.UsersComponent)
  },

  // PRODUCTS
  {
    path: 'products',
    loadComponent: () =>
      import('./modules/products/product.component')
        .then(m => m.ProductComponent)
  },

  // VEHICLES
  {
    path: 'vehicles',
    loadComponent: () =>
      import('./modules/vehicles/vehicle.component')
        .then(m => m.VehicleComponent)
  },

  // WAREHOUSES
  {
    path: 'warehouses',
    loadComponent: () =>
      import('./modules/warehouses/warehouse.component')
        .then(m => m.WarehouseComponent)
  },

  // INVENTORY
  {
    path: 'inventories',
    loadComponent: () =>
      import('./modules/inventory/inventory.component')
        .then(m => m.InventoryComponent)
  },

  // ✅ PICKING (ONLY ONE ROUTE – THIS IS IMPORTANT)
{
  path: 'inventory/picking/:shipmentId',
  loadComponent: () =>
    import('./modules/inventory/picking/picking.component')
      .then(m => m.PickingComponent),
  canActivate: [AuthGuard]
},

  // SHIPMENTS
  {
    path: 'shipments',
    loadComponent: () =>
      import('./modules/shipments/shipment.component')
        .then(m => m.ShipmentComponent)
  },

  // DISTANCES
  {
    path: 'distances',
    loadComponent: () =>
      import('./modules/distance/distance.component')
        .then(m => m.DistanceComponent)
  },

  
  // FALLBACK
  { path: '**', redirectTo: 'dashboard' }
];
