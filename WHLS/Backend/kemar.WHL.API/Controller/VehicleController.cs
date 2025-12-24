using Kemar.WHL.API.Common;
using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _service;

    public VehicleController(IVehicleService service)
    {
        _service = service;
    }

    [HttpPost("add-or-update")]
    public async Task<IActionResult> AddOrUpdate([FromBody] VehicleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid model");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var username = User.Identity!.Name!;

        var result = await _service.AddOrUpdateAsync(request, userId, username);
        return CommonHelper.ReturnActionResultByStatus(result, this);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        return CommonHelper.ReturnActionResult(response, this);
    }

    [HttpPost("filter")]
    public async Task<IActionResult> GetByFilter([FromBody] VehicleFilterRequest filter)
    {
        var response = await _service.GetByFilterAsync(filter);
        return CommonHelper.ReturnActionResult(response, this);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result)
            return NotFound("Vehicle not found");

        return Ok("Vehicle deleted successfully");
    }
}