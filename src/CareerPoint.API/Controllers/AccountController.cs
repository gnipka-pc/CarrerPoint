using AutoMapper;
using CareerPoint.Application.Services;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Enums;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerPoint.Web.Controllers;

/// <summary>
/// Контроллер пользователей
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    readonly IAuthAppService _authAppService;
    readonly IUserAppService _userAppService;
    readonly IMapper _mapper;

    /// <summary>
    /// Базовый конструктор контроллера пользователей
    /// </summary>
    /// <param name="authAppService">Апп сервис аутентификации</param>
    /// <param name="userAppService">Апп сервис пользователей</param>
    /// <param name="mapper">Автомаппер</param>
    public AccountController(
        IAuthAppService authAppService,
        IUserAppService userAppService,
        IMapper mapper)
    {
        _authAppService = authAppService;
        _userAppService = userAppService;
        _mapper = mapper;
    }

    /// <summary>
    /// Возвращает пользователя по его Id
    /// </summary>
    /// <returns>Пользователь</returns>
    [Authorize]
    [HttpGet("get-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByIdAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        UserDto? user = _mapper.Map<UserDto>(await _userAppService.GetUserByIdAsync(Guid.Parse(id)));

        if (user != null)
        {
            return Ok(user);
        }

        return NotFound("Пользователь не был найден");
    }

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("delete-account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        User? user = await _userAppService.GetUserByIdAsync(Guid.Parse(id));

        if (user is null)
            return NotFound("Пользователь не найден");

        await _userAppService.DeleteUserAsync(user);

        return Ok("Пользователь успешно удален");
    }

    /// <summary>
    /// Обновляет пользователя
    /// </summary>
    /// <param name="userDto">Пользователь</param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("update-account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAccountAsync([FromBody] UserDto userDto)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        User? user = await _userAppService.GetUserByIdAsync(Guid.Parse(id));

        if (user is null)
            return NotFound("Пользователь не найден");

        await _userAppService.UpdateUserAsync(_mapper.Map<User>(userDto));

        return Ok("Пользователь успешно изменен");
    }

    /// <summary>
    /// Добавляет ивент пользователю по айди
    /// </summary>
    /// <param name="eventId">Айди ивента</param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("add-event-to-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Удаляет ивент у пользователя по айди
    /// </summary>
    /// <param name="eventId">Айди ивента</param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("remove-event-from-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Получает ивенты пользователя
    /// </summary>
    /// <returns>Список ивентов</returns>
    [Authorize]
    [HttpGet("get-user-events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserEventsAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        List<Event> events = await _userAppService.GetUserEventsAsync(Guid.Parse(id));

        if (events.Count == 0)
            return NotFound("У пользователя нет ивентов");

        return Ok(_mapper.Map<List<EventDto>>(events));
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns></returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] UserDto user)
    {
        if (!(await _userAppService.GetUsersAsync())
            .Any(u => u.Username == user.Username || u.Email == user.Email || u.Id == user.Id))
        {
            await _userAppService.CreateUserAsync(_mapper.Map<User>(user));

            return Ok("Пользователь успешно добавлен");
        }

        return BadRequest("Пользователь с данной почтой или логином уже существует");
    }

    /// <summary>
    /// Вход в аккаунт
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException">Если роль не существует, то выбрасывается ошибка</exception>
    [HttpPost("sign-in")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        User? user = await _authAppService.FindUserByEmailAndPasswordAsync(request.Email, request.Password);

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

    /// <summary>
    /// Выход из аккаунта
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("sign-out")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok("Вы успешно вышли из аккаунта");
    }
}

/// <summary>
/// Запрос на вход в аккаунт
/// </summary>
/// <param name="Email">Почта</param>
/// <param name="Password">Пароль</param>
public record SignInRequest(string Email, string Password);