using kemar.WHL.Model.Filter;
using Kemar.WHL.API.Common;
using Kemar.WHL.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _service;

    public ShipmentController(IShipmentService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,TransportAdmin")]
    [HttpPost("add-or-update")]
    public async Task<IActionResult> AddOrUpdate([FromBody] ShipmentRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var username = User.Identity.Name;

        var result = await _service.AddOrUpdateAsync(request, userId, username);
        return CommonHelper.ReturnActionResultByStatus(result, this);
    }

    [Authorize(Roles = "Admin,TransportAdmin,WarehouseStaff,DeliveryStaff")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        return CommonHelper.ReturnActionResult(response, this);
    }

    [Authorize(Roles = "Admin,TransportAdmin,WarehouseStaff,DeliveryStaff")]
    [HttpPost("filter")]
    public async Task<IActionResult> GetByFilter([FromBody] ShipmentFilterModel filter)
    {
        var response = await _service.GetByFilterAsync(filter);
        return CommonHelper.ReturnActionResult(response, this);
    }

    [Authorize(Roles = "Admin,TransportAdmin,WarehouseStaff,DeliveryStaff")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return CommonHelper.ReturnActionResult(response, this);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result)
            return NotFound("Shipment not found");

        return Ok("Shipment deleted successfully");
    }
}