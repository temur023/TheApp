using Clean.Application.Abstractions;
using Clean.Application.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace loginForm.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService service):ControllerBase
{
    
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] UserFilter filter)
    {
        var response = await service.GetAll(filter);
        if (response.StatusCode != 200)
        {
            return StatusCode(response.StatusCode, response);
        }
        return Ok(response);
    }
    [HttpDelete("delete-selected")]
    public async Task<IActionResult> DeleteSelected(List<int> ids)
    {
        var response = await service.DeleteSelected(ids);
        if (response.StatusCode != 200)
        {
            return StatusCode(response.StatusCode, response);
        }
        return Ok(response);
    }
    [HttpPut("block-selected")]
    public async Task<IActionResult> BlockSelected(List<int> ids)
    {
        var response = await service.BlockSelected(ids);
        if (response.StatusCode != 200)
        {
            return StatusCode(response.StatusCode, response);
        }
        return Ok(response);
    }
    [HttpPut("unblock-selected")]
    public async Task<IActionResult> UnblockSelected(List<int> ids)
    {
        var response = await service.UnblockSelected(ids);
        if (response.StatusCode != 200)
        {
            return StatusCode(response.StatusCode, response);
        }
        return Ok(response);
    }

    [HttpDelete("delete-unverified")]
    public async Task<IActionResult> DeleteUnverified()
    {
        var response = await service.DeleteUnverified();
        if (response.StatusCode != 200)
        {
            return StatusCode(response.StatusCode, response);
        }
        return Ok(response);
    }
}