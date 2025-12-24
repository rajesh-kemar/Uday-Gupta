using Kemar.WHL.API.Common;
using Kemar.WHL.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,WarehouseStaff")]
    [HttpPost("add-or-update")]
    public async Task<IActionResult> AddOrUpdate([FromBody] InventoryRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid model");

        var username = User.Identity!.Name!;
        var result = await _service.AddOrUpdateAsync(request, username);
        return CommonHelper.ReturnActionResultByStatus(result, this);
    }

    [Authorize(Roles = "Admin,WarehouseStaff")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        if (response == null)
            return NotFound("Inventory not found");

        return Ok(response);
    }

    [Authorize(Roles = "Admin,WarehouseStaff")]
    [HttpPost("filter")]
    public async Task<IActionResult> GetByFilter([FromBody] InventoryFilterModel filter)
    {
        var response = await _service.GetByFilterAsync(filter);
        return CommonHelper.ReturnActionResult(response, this);
    }

    [Authorize(Roles = "Admin,WarehouseStaff")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var username = User.Identity!.Name!;
        var deleted = await _service.DeleteAsync(id, username);

        if (!deleted)
            return NotFound("Inventory not found");

        return Ok("Inventory deleted successfully");
    }
}
