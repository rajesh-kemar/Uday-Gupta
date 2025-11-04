using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Logistics.Api.Data;
using Logistics.Api.Models;

namespace Logistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly LogisticsDbContext _db;
        public VehiclesController(LogisticsDbContext db) => _db = db;
        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool? available = null)
        {
            var q = _db.Vehicles.AsQueryable();
            if (available.HasValue)
                q = q.Where(v => v.IsAvailable == available.Value);
            return Ok(await q.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var v = await _db.Vehicles.FindAsync(id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle v)
        {
            _db.Vehicles.Add(v);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = v.Id }, v);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Vehicle v)
        {
            if (id != v.Id) return BadRequest();
            _db.Entry(v).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _db.Vehicles.FindAsync(id);
            if (v == null) return NotFound();
            _db.Vehicles.Remove(v);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
