using Kemar.WHL.API.Common;
using Kemar.WHL.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpPost("add-or-update")]
    public async Task<IActionResult> AddOrUpdate([FromBody] ProductRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var username = User.Identity!.Name!;

        var result = await _service.AddOrUpdateAsync(request, userId, username);
        return CommonHelper.ReturnActionResultByStatus(result, this);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        if (response == null)
            return NotFound("Product not found");

        return Ok(response);
    }

    [HttpPost("filter")]
    public async Task<IActionResult> Filter([FromBody] ProductFilterRequest filter)
    {
        var result = await _service.GetByFilterAsync(filter);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var username = User.Identity!.Name!;
        var result = await _service.DeleteAsync(id, username);
        return CommonHelper.ReturnActionResultByStatus(result, this);
    }
}