using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Logistics.Api.Data;
using Logistics.Api.DTOs;
using Logistics.Api.Models;

namespace Logistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly LogisticsDbContext _db;

        public TripsController(LogisticsDbContext db)
        {
            _db = db;
        }

        // ------------------------- GET ALL TRIPS -------------------------
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? status = null)
        {
            var query = _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .AsQueryable();

            // Optional filtering by status (Planned, Active, Completed)
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TripStatus>(status, true, out var tripStatus))
                query = query.Where(t => t.Status == tripStatus);

            var trips = await query.ToListAsync();
            return Ok(trips);
        }

        // ------------------------- GET TRIP BY ID -------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var trip = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                return NotFound();

            return Ok(trip);
        }

        // ------------------------- CREATE TRIP -------------------------
        [HttpPost]
        public async Task<IActionResult> Create(CreateTripDto dto)
        {
            var driver = await _db.Drivers.FindAsync(dto.DriverId);
            var vehicle = await _db.Vehicles.FindAsync(dto.VehicleId);

            if (driver == null || vehicle == null)
                return BadRequest("Driver or vehicle not found.");

            if (!vehicle.IsAvailable)
                return BadRequest("Vehicle is not available.");

            var trip = new Trip
            {
                DriverId = dto.DriverId,
                VehicleId = dto.VehicleId,
                Origin = dto.Origin,
                Destination = dto.Destination,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = TripStatus.Planned
            };

            _db.Trips.Add(trip);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = trip.Id }, trip);
        }

        // ------------------------- START TRIP -------------------------
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartTrip(int id)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            var trip = await _db.Trips.Include(t => t.Vehicle).FirstOrDefaultAsync(t => t.Id == id);
            if (trip == null)
                return NotFound();

            if (trip.Status != TripStatus.Planned)
                return BadRequest("Trip is not in a planned state.");

            if (!trip.Vehicle!.IsAvailable)
                return BadRequest("Vehicle is currently not available.");

            trip.StartTime = DateTime.UtcNow;
            trip.Status = TripStatus.Active;
            trip.Vehicle.IsAvailable = false;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(trip);
        }

        // ------------------------- COMPLETE TRIP -------------------------
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTrip(int id)
        {
            var trip = await _db.Trips.Include(t => t.Vehicle).FirstOrDefaultAsync(t => t.Id == id);
            if (trip == null)
                return NotFound();

            if (trip.Status != TripStatus.Active)
                return BadRequest("Trip is not active.");

            trip.EndTime = DateTime.UtcNow;
            trip.Status = TripStatus.Completed;
            trip.Vehicle!.IsAvailable = true;

            await _db.SaveChangesAsync();
            return Ok(trip);
        }

        // ------------------------- UPDATE STATUS (MANUAL) -------------------------
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] TripStatus status)
        {
            var trip = await _db.Trips.Include(t => t.Vehicle).FirstOrDefaultAsync(t => t.Id == id);
            if (trip == null)
                return NotFound();

            trip.Status = status;

            // Keep vehicle availability in sync
            if (status == TripStatus.Completed)
                trip.Vehicle!.IsAvailable = true;
            else if (status == TripStatus.Active)
                trip.Vehicle!.IsAvailable = false;

            await _db.SaveChangesAsync();
            return Ok(trip);
        }

        // ------------------------- GET TRIPS LONGER THAN X HOURS -------------------------
        [HttpGet("longer-than/{hours:int}")]
        public async Task<IActionResult> LongerThan(int hours)
        {
            var trips = await _db.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .Where(t => t.StartTime.HasValue && t.EndTime.HasValue)
                .Where(t => EF.Functions.DateDiffHour(t.StartTime.Value, t.EndTime.Value) > hours)
                .ToListAsync();

            return Ok(trips);
        }

        // ------------------------- DELETE TRIP -------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var trip = await _db.Trips.FindAsync(id);
            if (trip == null)
                return NotFound();

            _db.Trips.Remove(trip);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
