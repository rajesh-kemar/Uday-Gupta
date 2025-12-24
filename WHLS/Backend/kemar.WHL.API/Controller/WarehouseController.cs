using Kemar.WHL.API.Common;
using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _service;

    public WarehouseController(IWarehouseService service)
    {
        _service = service;
    }

    [HttpPost("add-or-update")]
    public async Task<IActionResult> AddOrUpdate([FromBody] WarehouseRequest request)
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

        if (response == null)
            return NotFound("Warehouse not found");

        return Ok(response);
    }

    [HttpPost("filter")]
    public async Task<IActionResult> GetByFilter([FromBody] WarehouseFilterModel filter)
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
            return NotFound("Warehouse not found");

        return Ok("Warehouse deleted successfully");
    }
}