import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TripFormComponent } from './components/trip-form/trip-form.component';
import { TripListComponent } from './components/trip-list/trip-list.component';
imports: [CommonModule, TripFormComponent, TripListComponent]


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    TripFormComponent,
    TripListComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {}
