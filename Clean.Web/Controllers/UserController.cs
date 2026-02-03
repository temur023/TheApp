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
        return Ok(await service.GetAll(filter));
    }
    [HttpDelete("delete-selected")]
    public async Task<IActionResult> DeleteSelected(List<int> ids)
    {
        return Ok(await service.DeleteSelected(ids));
    }
    [HttpPut("block-selected")]
    public async Task<IActionResult> BlockSelected(List<int> ids)
    {
        return Ok(await service.BlockSelected(ids));
    }
    [HttpPut("unblock-selected")]
    public async Task<IActionResult> UnblockSelected(List<int> ids)
    {
        return Ok(await service.UnblockSelected(ids));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok(await service.Delete(id));
    }

    [HttpDelete("delete-unverified")]
    public async Task<IActionResult> DeleteUnverified()
    {
        return Ok(await service.DeleteUnverified());
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Block(int id)
    {
        return Ok(await service.Block(id));
    }
}