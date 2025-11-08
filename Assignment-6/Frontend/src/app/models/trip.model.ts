export interface Trip {
  id?: number;
  driverId: number;
  vehicleId: number;
  source: string;         // new
  destination: string;
  startingDate: string;   // new
  endingDate: string;     // new
  isCompleted: boolean;
  driver?: { id: number; name: string };
  vehicle?: { id: number; plateNumber: string };
}
