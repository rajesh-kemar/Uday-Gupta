using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripApiEF.Data;
using TripApiEF.Models;
using TripApiEF.DTOs;

namespace TripApiEF.Controllers
{
    [Route("trips")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly TripDbContext _context;

        public TripsController(TripDbContext context)
        {
            _context = context;
        }

        // ✅ GET /trips
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _context.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .ToListAsync();

            return Ok(trips);
        }

        // ✅ POST /trips
        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] TripCreateDto tripDto)
        {
            var driver = await _context.Drivers.FindAsync(tripDto.DriverId);
            if (driver == null)
                return BadRequest($"Driver with ID {tripDto.DriverId} does not exist.");

            var vehicle = await _context.Vehicles.FindAsync(tripDto.VehicleId);
            if (vehicle == null)
                return BadRequest($"Vehicle with ID {tripDto.VehicleId} does not exist.");

            var trip = new Trip
            {
                DriverId = tripDto.DriverId,
                VehicleId = tripDto.VehicleId,
                Source = tripDto.Source,
                Destination = tripDto.Destination,
                StartingDate = tripDto.StartingDate,
                EndingDate = tripDto.EndingDate,
                IsCompleted = tripDto.IsCompleted
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrips), new { id = trip.Id }, trip);
        }

        // ✅ PUT /trips/{id} → Mark trip as completed
        [HttpPut("{id}")]
        public async Task<IActionResult> MarkTripCompleted(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return NotFound($"Trip with ID {id} not found.");

            trip.IsCompleted = true;
            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // ✅ GET /trips/available-vehicles → List vehicles not in completed trips
        [HttpGet("available-vehicles")]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            var usedVehicleIds = await _context.Trips
                .Where(t => !t.IsCompleted)
                .Select(t => t.VehicleId)
                .ToListAsync();

            var availableVehicles = await _context.Vehicles
                .Where(v => !usedVehicleIds.Contains(v.Id))
                .ToListAsync();

            return Ok(availableVehicles);
        }
    }
}
