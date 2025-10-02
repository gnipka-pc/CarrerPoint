using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;

namespace CareerPoint.Web.Controllers;

[Route("api/users")]
public class UserController : ControllerBase
{
    readonly IUserAppService _userAppService;

    public UserController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    [HttpGet("get-user/{id}")]
    public async Task<IActionResult> GetUserByIdAsync(Guid id)
    {
        return Ok(await _userAppService.GetUserByIdAsync(id));
    }

    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsersAsync()
    {
        return Ok(await _userAppService.GetUsersAsync());
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserDto userDto)
    {
        await _userAppService.CreateUserAsync(userDto);

        return Ok("Пользователь успешно добавлен");
    }

    [HttpDelete("delete-user/{id}")]
    public async Task<IActionResult> DeleteUserAsync(Guid id)
    {
        await _userAppService.DeleteUserAsync(id);

        return Ok("Пользователь успешно удален");
    }

    [HttpPut("update-user")]
    public async Task<IActionResult> UpdateUserAsync(UserDto userDto)
    {
        await _userAppService.UpdateUserAsync(userDto);

        return Ok("Пользователь успешно изменен");
    }
}
