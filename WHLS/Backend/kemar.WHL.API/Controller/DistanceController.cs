using Kemar.WHL.API.Common;
using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kemar.WHL.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class DistanceController : ControllerBase
    {
        private readonly IDistanceService _service;

        public DistanceController(IDistanceService service)
        {
            _service = service;
        }

        [HttpPost("add-or-update")]
        public async Task<IActionResult> AddOrUpdate([FromBody] DistanceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            var username = User.Identity?.Name ?? "System"; 
            var result = await _service.AddOrUpdateAsync(request, username);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return CommonHelper.ReturnActionResult(response, this);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetByFilter([FromBody] DistanceFilterModel filter)
        {
            var response = await _service.GetByFilterAsync(filter);
            return CommonHelper.ReturnActionResult(response, this);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return CommonHelper.ReturnActionResult(result, this);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var username = User.Identity?.Name ?? "System";
            var deleted = await _service.DeleteAsync(id, username);
            if (!deleted)
                return NotFound("Distance not found");

            return Ok("Distance deleted successfully");
        }
    }
}