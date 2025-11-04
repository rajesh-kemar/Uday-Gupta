using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripApiEF.Data;
using TripApiEF.Models;

[Route("vehicles")]
[ApiController]
public class VehiclesController : ControllerBase
{
    private readonly TripDbContext _context;

    public VehiclesController(TripDbContext context)
    {
        _context = context;
    }

    // GET /vehicles
    [HttpGet]
    public async Task<IActionResult> GetVehicles()
    {
        var vehicles = await _context.Vehicles
            .Include(v => v.Trips) // optional
            .ToListAsync();

        return Ok(vehicles);
    }

    // POST /vehicles
    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVehicles), new { id = vehicle.Id }, vehicle);
    }
}
