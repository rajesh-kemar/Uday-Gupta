using Kemar.WHL.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PickingController : ControllerBase
{
    private readonly IPickingService _service;

    public PickingController(IPickingService service)
    {
        _service = service;
    }

    [Authorize(Roles = "WarehouseStaff,Admin")]
    [HttpPost("pick")]
    public async Task<IActionResult> Pick([FromBody] PickingRequest request)
    {
        var username = User.Identity!.Name!;
        var result = await _service.PickAsync(request, username);
        return CommonHelper.ReturnActionResultByStatus(result, this);
    }

    [Authorize(Roles = "WarehouseStaff,Admin")]
    [HttpGet("by-shipment/{shipmentId}")]
    public async Task<IActionResult> GetByShipment(int shipmentId)
    {
        var result = await _service.GetByShipmentAsync(shipmentId);
        return Ok(result);
    }
}

