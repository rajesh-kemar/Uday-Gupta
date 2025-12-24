using kemar.WHL.Model.Filter;
using Kemar.WHL.API.Common;
using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Repository.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    private int GetLoggedInUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        return userIdClaim == null ? 0 : int.Parse(userIdClaim);
    }

    [HttpPost("add-or-update")]
    public async Task<IActionResult> AddOrUpdate([FromBody] UserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid model");

        int loggedInUserId = GetLoggedInUserId();
        var result = await _service.AddOrUpdateAsync(request, loggedInUserId);

        return CommonHelper.ReturnActionResultByStatus(result, this);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound("User not found");

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpPost("filter")]
    public async Task<IActionResult> Filter([FromBody] UserFilterModel filter)
    {
        var result = await _service.GetByFilterAsync(filter);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        int loggedInUserId = GetLoggedInUserId();
        var success = await _service.DeleteAsync(id, loggedInUserId);

        if (!success)
            return NotFound("User not found");

        return Ok("User deleted successfully");
    }
}
