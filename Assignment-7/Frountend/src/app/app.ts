import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  template: `
    <header class="navbar">
      <div class="logo">
        🚚 <span>Logistics Management System</span>
      </div>
      <nav class="menu">
        <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Dashboard</a>
        <a routerLink="/drivers" routerLinkActive="active">Drivers</a>
        <a routerLink="/vehicles" routerLinkActive="active">Vehicles</a>
        <a routerLink="/trips" routerLinkActive="active">Trips</a>
      </nav>
    </header>

    <main class="container">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .navbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: #ffffff;
      padding: 15px 30px;
      border-bottom: 2px solid #f0f0f0;
      box-shadow: 0 2px 5px rgba(0,0,0,0.05);
    }

    .logo {
      font-size: 20px;
      font-weight: bold;
      color: #222;
    }

    .menu a {
      margin: 0 15px;
      color: #007bff;
      text-decoration: none;
      font-weight: 500;
      transition: color 0.3s, border-bottom 0.3s;
    }

    .menu a:hover {
      color: #0056b3;
    }

    .menu a.active {
      color: #000;
      border-bottom: 2px solid #000;
    }

    .container {
      padding: 30px;
      background-color: #fafafa;
      min-height: calc(100vh - 80px);
    }
  `]
})
export class AppComponent {}
