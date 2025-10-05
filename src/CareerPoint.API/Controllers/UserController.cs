using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;

namespace CareerPoint.Web.Controllers;

[Route("api/users")]
[ApiController]
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
        UserDto? user = await _userAppService.GetUserByIdAsync(id);

        if (user != null)
        {
            return Ok(user);
        }

        return NotFound("Пользователь не был найден");
    }

    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsersAsync()
    {
        return Ok(await _userAppService.GetUsersAsync());
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserDto userDto)
    {
        if (await _userAppService.GetUserByIdAsync(userDto.Id) == null)
        {
            await _userAppService.CreateUserAsync(userDto);

            return Ok("Пользователь успешно добавлен");
        }
        
        return BadRequest("Пользователь с данным Id уже существует");
    }

    [HttpDelete("delete-user/{id}")]
    public async Task<IActionResult> DeleteUserAsync(Guid id)
    {
        UserDto? user = await _userAppService.GetUserByIdAsync(id);

        if (user != null)
        {
            await _userAppService.DeleteUserAsync(user);

            return Ok("Пользователь успешно удален");
        }

        return BadRequest("Пользователь не был найден");
    }

    [HttpPut("update-user")]
    public async Task<IActionResult> UpdateUserAsync(UserDto userDto)
    {
        if (await _userAppService.GetUserByIdAsync(userDto.Id) != null)
        {
            await _userAppService.UpdateUserAsync(userDto);

            return Ok("Пользователь успешно изменен");
        }

        return BadRequest("Пользователь не был найден");        
    }
}
