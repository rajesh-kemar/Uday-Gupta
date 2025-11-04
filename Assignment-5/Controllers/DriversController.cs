using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripApiEF.Data;
using TripApiEF.Models;

namespace TripApiEF.Controllers
{
    [Route("drivers")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly TripDbContext _context;

        public DriversController(TripDbContext context)
        {
            _context = context;
        }

        // GET /drivers
        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            var drivers = await _context.Drivers
                .Include(d => d.Trips)
                .ThenInclude(t => t.Vehicle)
                .ToListAsync();

            return Ok(drivers);
        }

        // POST /drivers
        [HttpPost]
        public async Task<IActionResult> CreateDriver([FromBody] Driver driver)
        {
            // Avoid validation errors by removing trips from incoming JSON
            driver.Trips = null;

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDrivers), new { id = driver.Id }, driver);
        }
        // DELETE /drivers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound($"Driver with ID {id} not found."); // Corrected message

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
