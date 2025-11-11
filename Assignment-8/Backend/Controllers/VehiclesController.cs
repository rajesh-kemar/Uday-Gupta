using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvLogisticSystem.Data;
using AdvLogisticSystem.Models;

namespace AdvLogisticSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly LodisticsDbContext _db;
        public VehiclesController(LodisticsDbContext db) { _db = db; }

        // GET: api/vehicles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehicles = await _db.Vehicles.AsNoTracking().ToListAsync();
            return Ok(vehicles);
        }

        // GET: api/vehicles/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var v = await _db.Vehicles.FindAsync(id);
            if (v == null) return NotFound(new { message = $"Vehicle with ID {id} not found" });
            return Ok(v);
        }

        // POST: api/vehicles
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vehicle vehicle)
        {
            if (vehicle == null)
                return BadRequest(new { message = "Invalid vehicle data" });

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
        }

        // PUT: api/vehicles/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Vehicle updatedVehicle)
        {
            if (updatedVehicle == null)
                return BadRequest(new { message = "Invalid vehicle data" });

            if (id != updatedVehicle.Id)
                return BadRequest(new { message = "Vehicle ID mismatch" });

            var existing = await _db.Vehicles.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Vehicle with ID {id} not found" });

            // apply updates
            existing.RegistrationNumber = updatedVehicle.RegistrationNumber;
            existing.Model = updatedVehicle.Model;
            existing.IsAvailable = updatedVehicle.IsAvailable;

            // Force EF to treat entity as modified (avoids SaveChangesAsync returning 0)
            _db.Entry(existing).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Vehicle updated successfully", vehicle = existing });
        }

        // DELETE: api/vehicles/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Vehicles.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Vehicle with ID {id} not found" });

            _db.Vehicles.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Vehicle deleted successfully" });
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            var availableVehicles = await _db.Vehicles
                .Where(v => v.IsAvailable == true)
                .ToListAsync();

            return Ok(availableVehicles);
        }

    }
}
