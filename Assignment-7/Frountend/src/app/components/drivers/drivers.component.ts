import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ApiService } from '../../services/api';

interface Driver {
  id: number;
  name: string;
  licenseNumber: string;
  phone: string;
}

@Component({
  selector: 'app-drivers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './drivers.component.html',
  styleUrls: ['./drivers.component.css']
})
export class DriversComponent implements OnInit {
  drivers: Driver[] = [];

  // ✅ Form model
  newDriver: Partial<Driver> = {
    name: '',
    licenseNumber: '',
    phone: ''
  };

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadDrivers();
  }

  /** ✅ Load all drivers from backend */
  loadDrivers(): void {
    this.api.getDrivers().subscribe({
      next: (data) => {
        this.drivers = data.map((d: any) => ({
          id: d.id,
          name: d.name,
          licenseNumber: d.licenseNumber,
          phone: d.phone
        }));
      },
      error: (err) => console.error('❌ Failed to load drivers:', err)
    });
  }

  /** ✅ Add new driver with validation */
  addDriver(form: NgForm): void {
    if (form.invalid) {
      alert('⚠️ Please fill all required fields.');
      return;
    }

    const payload = {
      name: this.newDriver.name?.trim(),
      licenseNumber: this.newDriver.licenseNumber?.trim(),
      phone: this.newDriver.phone?.trim()
    };

    this.api.addDriver(payload).subscribe({
      next: (driver) => {
        alert('✅ Driver added successfully!');
        this.drivers.push(driver);
        form.resetForm();
        this.newDriver = { name: '', licenseNumber: '', phone: '' };
      },
      error: (err) => {
        console.error('❌ Failed to add driver:', err);
        alert('❌ Failed to add driver. Please try again.');
      }
    });
  }
}
