using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvLogisticSystem.Data;
using AdvLogisticSystem.Models;

namespace AdvLogisticSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly LodisticsDbContext _db;

        public DriversController(LodisticsDbContext db)
        {
            _db = db;
        }

        // ✅ GET: api/drivers
        // Get all drivers (for management page)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var drivers = await _db.Drivers
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .ToListAsync();

            return Ok(drivers);
        }

        // ✅ GET: api/drivers/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound(new { message = $"Driver with ID {id} not found" });

            return Ok(driver);
        }

        // ✅ POST: api/drivers
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Driver driver)
        {
            if (driver == null)
                return BadRequest(new { message = "Invalid driver data" });

            // Default to available if not specified
            driver.IsAvailable = driver.IsAvailable;

            _db.Drivers.Add(driver);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
        }

        // ✅ PUT: api/drivers/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Driver updatedDriver)
        {
            if (updatedDriver == null)
                return BadRequest(new { message = "Invalid driver data" });

            if (id != updatedDriver.Id)
                return BadRequest(new { message = "Driver ID mismatch" });

            var existingDriver = await _db.Drivers.FindAsync(id);
            if (existingDriver == null)
                return NotFound(new { message = $"Driver with ID {id} not found" });

            // ✅ Update all fields including Experience
            existingDriver.Name = updatedDriver.Name;
            existingDriver.LicenseNumber = updatedDriver.LicenseNumber;
            existingDriver.Phone = updatedDriver.Phone;
            existingDriver.Experience = updatedDriver.Experience;  // <-- 🔥 This line was missing
            existingDriver.IsAvailable = updatedDriver.IsAvailable;

            _db.Entry(existingDriver).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver updated successfully",
                driver = existingDriver
            });
        }


        // ✅ DELETE: api/drivers/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound(new { message = $"Driver with ID {id} not found" });

            _db.Drivers.Remove(driver);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Driver deleted successfully" });
        }

        // ✅ GET: api/drivers/available
        // This endpoint is used by the Trip form to show only available drivers
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableDrivers()
        {
            var availableDrivers = await _db.Drivers
                .Where(d => d.IsAvailable == true)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.LicenseNumber,
                    d.Phone
                })
                .OrderBy(d => d.Name)
                .ToListAsync();

            return Ok(new
            {
                message = "Available drivers fetched successfully",
                count = availableDrivers.Count,
                data = availableDrivers
            });
        }
    }
}
