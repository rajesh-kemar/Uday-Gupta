import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ApiService } from '../../services/api';

interface Vehicle {
  id: number;
  vehicleNumber: string;
  model: string;
  capacity: number;
  isAvailable: boolean;
}

@Component({
  selector: 'app-vehicles',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.css']
})
export class VehiclesComponent implements OnInit {
  vehicles: Vehicle[] = [];

  // Form fields
  newVehicleNumber: string = '';
  newModel: string = '';
  newCapacity: number | null = null;
  newIsAvailable: boolean = true;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadVehicles();
  }

  /** ✅ Load vehicles from backend */
  loadVehicles(): void {
    this.api.getVehicles().subscribe({
      next: (data) => (this.vehicles = data),
      error: (err) => console.error('❌ Failed to load vehicles:', err)
    });
  }

  /** ✅ Add a new vehicle with form validation */
  addVehicle(form: NgForm): void {
    if (form.invalid) return;

    const newVehicle = {
      VehicleNumber: this.newVehicleNumber.trim(),
      Model: this.newModel.trim(),
      Capacity: this.newCapacity ?? 0,
      IsAvailable: this.newIsAvailable
    };

    this.api.addVehicle(newVehicle).subscribe({
      next: (v) => {
        this.vehicles.push(v);
        form.resetForm({ newIsAvailable: true }); // Reset the form
        console.log('✅ Vehicle added successfully');
      },
      error: (err) => console.error('❌ Failed to add vehicle:', err)
    });
  }

  /** ✅ Toggle vehicle availability and update backend */
  toggleAvailability(v: Vehicle): void {
    v.isAvailable = !v.isAvailable;

    const updatedVehicle = {
      Id: v.id,
      VehicleNumber: v.vehicleNumber,
      Model: v.model,
      Capacity: v.capacity,
      IsAvailable: v.isAvailable
    };

    this.api.updateVehicle(updatedVehicle).subscribe({
      next: () => console.log('✅ Vehicle availability updated'),
      error: (err) => console.error('❌ Failed to update vehicle:', err)
    });
  }
}
