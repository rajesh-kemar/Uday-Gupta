using AdvLogisticSystem.Data;
using AdvLogisticSystem.DTOs;
using AdvLogisticSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AdvLogisticSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly LodisticsDbContext _db;

        public TripsController(LodisticsDbContext db)
        {
            _db = db;
        }

        // ✅ GET: api/Trips
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .AsNoTracking()
                .ToListAsync();

            return Ok(trips);
        }

        // ✅ GET: api/Trips/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var trip = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                return NotFound(new { message = $"Trip with ID {id} not found" });

            return Ok(trip);
        }

        // ✅ NEW ENDPOINT — GET: api/Trips/driver/{driverId}
        [HttpGet("driver/{driverId:int}")]
        public async Task<IActionResult> GetTripsByDriverId(int driverId)
        {
            var trips = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .Where(t => t.DriverId == driverId)
                .AsNoTracking()
                .ToListAsync();

            if (trips == null || !trips.Any())
                return NotFound(new { message = $"No trips found for driver ID {driverId}" });

            return Ok(trips);
        }

        // ✅ POST: api/Trips
        [Authorize(Roles = "Dispatcher")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTripDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var driver = await _db.Drivers.FindAsync(dto.DriverId);
            var vehicle = await _db.Vehicles.FindAsync(dto.VehicleId);

            if (driver == null) return BadRequest(new { message = "Invalid DriverId" });
            if (vehicle == null) return BadRequest(new { message = "Invalid VehicleId" });

            if (!driver.IsAvailable) return BadRequest(new { message = "Driver is already assigned." });
            if (!vehicle.IsAvailable) return BadRequest(new { message = "Vehicle is already assigned." });

            var trip = new Trip
            {
                DriverId = dto.DriverId,
                VehicleId = dto.VehicleId,
                Origin = dto.Origin ?? "",
                Destination = dto.Destination ?? "",
                StartTime = dto.StartTime ?? DateTime.Now,
                EndTime = null,
                Status = dto.Status ?? "Active"
            };

            // Update availability for Active trips
            if (trip.Status == "Active")
            {
                driver.IsAvailable = false;
                vehicle.IsAvailable = false;
            }

            _db.Trips.Add(trip);
            await _db.SaveChangesAsync();

            var createdTrip = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == trip.Id);

            return CreatedAtAction(nameof(Get), new { id = trip.Id }, createdTrip);
        }

        // ✅ PUT: api/Trips/5
        [Authorize(Roles = "Dispatcher")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTripDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trip = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                return NotFound(new { message = $"Trip with ID {id} not found" });

            // Get previous driver and vehicle (to toggle availability later)
            var prevDriverId = trip.DriverId;
            var prevVehicleId = trip.VehicleId;

            // Apply updates
            trip.DriverId = dto.DriverId ?? trip.DriverId;
            trip.VehicleId = dto.VehicleId ?? trip.VehicleId;
            trip.Origin = dto.Origin ?? trip.Origin;
            trip.Destination = dto.Destination ?? trip.Destination;
            trip.StartTime = dto.StartTime ?? trip.StartTime;
            trip.Status = dto.Status ?? trip.Status;

            // ✅ If Completed, mark EndTime and free resources
            if (dto.Status == "Completed")
            {
                trip.EndTime = dto.EndTime ?? DateTime.Now;

                var driver = await _db.Drivers.FindAsync(trip.DriverId);
                var vehicle = await _db.Vehicles.FindAsync(trip.VehicleId);

                if (driver != null) driver.IsAvailable = true;
                if (vehicle != null) vehicle.IsAvailable = true;
            }
            else if (dto.Status == "Active")
            {
                trip.EndTime = null;

                var driver = await _db.Drivers.FindAsync(trip.DriverId);
                var vehicle = await _db.Vehicles.FindAsync(trip.VehicleId);

                if (driver != null) driver.IsAvailable = false;
                if (vehicle != null) vehicle.IsAvailable = false;
            }
            else if (dto.Status == "Cancelled")
            {
                trip.EndTime = null;

                var driver = await _db.Drivers.FindAsync(trip.DriverId);
                var vehicle = await _db.Vehicles.FindAsync(trip.VehicleId);

                if (driver != null) driver.IsAvailable = true;
                if (vehicle != null) vehicle.IsAvailable = true;
            }

            // ✅ Handle previous driver/vehicle becoming free
            if (prevDriverId != trip.DriverId)
            {
                var prevDriver = await _db.Drivers.FindAsync(prevDriverId);
                if (prevDriver != null) prevDriver.IsAvailable = true;
            }

            if (prevVehicleId != trip.VehicleId)
            {
                var prevVehicle = await _db.Vehicles.FindAsync(prevVehicleId);
                if (prevVehicle != null) prevVehicle.IsAvailable = true;
            }

            // ✅ Force EF Core to track the entity and save properly
            _db.Trips.Update(trip);
            await _db.SaveChangesAsync();

            // Return updated entity with driver and vehicle included
            var updatedTrip = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            return Ok(updatedTrip);
        }

        // ✅ PUT: api/Trips/{id}/complete
        [Authorize(Roles = "Dispatcher")]
        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> MarkTripAsCompleted(int id)
        {
            var trip = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                return NotFound(new { message = $"Trip with ID {id} not found" });

            // Mark as completed
            trip.Status = "Completed";
            trip.EndTime = DateTime.Now;

            // Make driver and vehicle available again
            if (trip.Driver != null) trip.Driver.IsAvailable = true;
            if (trip.Vehicle != null) trip.Vehicle.IsAvailable = true;

            await _db.SaveChangesAsync();

            var updatedTrip = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            return Ok(updatedTrip);
        }

        // ✅ DELETE: api/Trips/5
        [Authorize(Roles = "Dispatcher")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var trip = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                return NotFound(new { message = $"Trip with ID {id} not found" });

            if (trip.Driver != null) trip.Driver.IsAvailable = true;
            if (trip.Vehicle != null) trip.Vehicle.IsAvailable = true;

            _db.Trips.Remove(trip);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Trip deleted successfully and driver/vehicle set available." });
        }

        // ✅ GET: api/Trips/summary/{driverId}
        [HttpGet("summary/{driverId:int}")]
        public async Task<IActionResult> GetDriverSummary(int driverId)
        {
            var param = new SqlParameter("@DriverId", driverId);

            var summaryList = await _db.DriverTripSummaries
                .FromSqlRaw("EXEC dbo.GetDriverTripSummary @DriverId", param)
                .ToListAsync();

            var result = summaryList.FirstOrDefault() ?? new DriverTripSummary
            {
                DriverId = driverId,
                TotalTrips = 0,
                TotalHoursDriven = 0
            };

            return Ok(result);
        }
    }
}