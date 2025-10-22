using CareerPoint.Application.Services;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Enums;
using CareerPoint.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerPoint.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    readonly IAuthAppService _authAppService;
    readonly IUserAppService _userAppService;

    public AccountController(
        IAuthAppService authAppService,
        IUserAppService userAppService)
    {
        _authAppService = authAppService;
        _userAppService = userAppService;
    }

    [Authorize]
    [HttpGet("get-user")]
    public async Task<IActionResult> GetUserByIdAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        UserDto? user = await _userAppService.GetUserByIdAsync(Guid.Parse(id));

        if (user != null)
        {
            return Ok(user);
        }

        return NotFound("Пользователь не был найден");
    }

    [Authorize]
    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        UserDto? user = await _userAppService.GetUserByIdAsync(Guid.Parse(id));

        if (user is null)
            return NotFound("Пользователь не найден");

        await _userAppService.DeleteUserAsync(user);

        return Ok("Пользователь успешно удален");
    }

    [Authorize]
    [HttpPut("update-account")]
    public async Task<IActionResult> UpdateAccountAsync([FromBody] UserDto user)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        await _userAppService.UpdateUserAsync(user);

        return Ok("Пользователь успешно изменен");
    }

    [Authorize]
    [HttpPut("add-event-to-user")]
    public async Task<IActionResult> AddEventToUserAsync([FromBody] Guid eventId)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        bool isSucсess = await _userAppService.AddEventToUserAsync(Guid.Parse(id), eventId);

        if (isSucсess)
            return Ok("Ивент успешно добавлен пользователю");

        return BadRequest("Не удалось добавить ивент пользователю");
    }

    [Authorize]
    [HttpPut("remove-event-from-user")]
    public async Task<IActionResult> RemoveEventFromUser([FromBody] Guid eventId)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        bool isSucess = await _userAppService.RemoveEventFromUserAsync(Guid.Parse(id), eventId);

        if (isSucess)
            return Ok("Ивент успешно удален у пользователя");

        return BadRequest("Не удалось удалить ивент у пользователя");
    }

    [Authorize]
    [HttpGet("get-user-events")]
    public async Task<IActionResult> GetUserEventsAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        List<EventDto> events = await _userAppService.GetUserEventsAsync(Guid.Parse(id));

        if (events.Count == 0)
            return NotFound("У пользователя нет ивентов");

        return Ok(events);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserDto user)
    {
        if (!(await _userAppService.GetUsersAsync())
            .Any(u => u.Username == user.Username || u.Email == user.Email || u.Id == user.Id))
        {
            await _userAppService.CreateUserAsync(user);

            return Ok("Пользователь успешно добавлен");
        }

        return BadRequest("Пользователь с данной почтой или логином уже существует");
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        UserDto? user = await _authAppService.FindUserByEmailAndPasswordAsync(request.Email, request.Password);

        if (user is null)
            return BadRequest("Почта или пароль не правильные");

        string userRole = user.UserRole switch
        {
            UserRole.Admin => "Admin",
            UserRole.Manager => "Manager",
            UserRole.DefaultUser => "DefaultUser",
            _ => throw new UnauthorizedAccessException()
        };

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, userRole)
        };

        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(2)
            });

        return Ok("Вы успешно вошли в аккаунт");
    }

    [Authorize]
    [HttpGet("sign-out")]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok("Вы успешно вышли из аккаунта");
    }

    //[HttpGet("get-claims")]
    //[Authorize(Roles = "Admin")]
    //public IActionResult GetClaims()
    //{
    //    return Ok(User.Claims.Select(c => new {c.Type, c.Value}));
    //}
}

public record SignInRequest(string Email, string Password);