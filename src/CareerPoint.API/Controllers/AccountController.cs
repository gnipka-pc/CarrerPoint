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
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using System.Data;
using System.Security.AccessControl;
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
    readonly IMinioClient _minioClient;
    readonly string bucketName = "avatars";

    /// <summary>
    /// Базовый конструктор контроллера пользователей
    /// </summary>
    /// <param name="authAppService">Апп сервис аутентификации</param>
    /// <param name="userAppService">Апп сервис пользователей</param>
    /// <param name="mapper">Автомаппер</param>
    /// <param name="minioClient">Minio клиент</param>
    public AccountController(
        IAuthAppService authAppService,
        IUserAppService userAppService,
        IMapper mapper,
        IMinioClient minioClient)
    {
        _authAppService = authAppService;
        _userAppService = userAppService;
        _mapper = mapper;
        _minioClient = minioClient;
    }

    /// <summary>
    /// Возвращает всех пользователей
    /// </summary>
    /// <returns>Список всех пользователей</returns>
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("get-all-users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        List<User> users = await _userAppService.GetUsersAsync();
        List<UserDto> userDtos = _mapper.Map<List<UserDto>>(users);

        return Ok(userDtos);
    }

    /// <summary>
    /// Возвращает пользователя по его Id
    /// </summary>
    /// <returns>Пользователь</returns>
    [Authorize]
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpGet("get-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByIdAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpDelete("delete-account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
    [Authorize(Roles = "Admin, DefaultUser")]
    [HttpPut("update-account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAccountAsync([FromBody] UserDto userDto)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        User? user = await _userAppService.GetUserByIdAsync(Guid.Parse(id));

        if (user is null)
            return NotFound("Пользователь не найден");

        var updateUser = _mapper.Map<User>(userDto);
        updateUser.HashedPassword = user.HashedPassword;

        await _userAppService.UpdateUserAsync(updateUser);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        await SignInWithClaimsAsync(user.UserRole, id);

        return Ok("Пользователь успешно изменен");
    }


    /// <summary>
    /// Добавляет ивент пользователю по айди
    /// </summary>
    /// <param name="eventId">Айди ивента</param>
    /// <returns></returns>
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpPut("add-event-to-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddEventToUserAsync([FromBody] Guid eventId)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpPut("remove-event-from-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveEventFromUser([FromBody] Guid eventId)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        bool isSucess = await _userAppService.RemoveEventFromUserAsync(Guid.Parse(id), eventId);

        if (isSucess)
            return Ok("Ивент успешно удален у пользователя");

        return BadRequest("Не удалось удалить ивент у пользователя");
    }

    /// <summary>
    /// Получает ивенты пользователя
    /// </summary>
    /// <returns>Список ивентов</returns>
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpGet("get-user-events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserEventsAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserDto user)
    {
        if (!(await _userAppService.GetUsersAsync())
            .Any(u => u.Username == user.Username || u.Email == user.Email))
        {
            var userEntity = _mapper.Map<User>(user);
            userEntity.UserRole = UserRole.DefaultUser;

            return Ok(await _userAppService.CreateUserAsync(userEntity));
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

        await SignInWithClaimsAsync(user.UserRole, user.Id.ToString());

        return Ok("Вы успешно вошли в аккаунт");
    }

    /// <summary>
    /// Выход из аккаунта
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpGet("sign-out")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok("Вы успешно вышли из аккаунта");
    }
    
    /// <summary>
    /// Удаление пользователя по ID (для менеджера)
    /// </summary>
    /// <param name="userId">ID пользователя для удаления</param>
    /// <returns></returns>
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpDelete("delete-user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAsync(Guid userId)
    {
        User? user = await _userAppService.GetUserByIdAsync(userId);

        if (user is null)
            return NotFound("Пользователь не найден");

        await _userAppService.DeleteUserAsync(user);

        return Ok("Пользователь успешно удален");
    }

    /// <summary>
    /// Обновление данных пользователя по ID (для менеджера)
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="userDto">Новые данные</param>
    /// <returns></returns>
    [Authorize(Roles = "Manager,Admin")]
    [HttpPut("update-user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserAsync(Guid userId, [FromBody] UserDto userDto)
    {
        // Проверяем, что пользователь существует
        User? existingUser = await _userAppService.GetUserByIdAsync(userId);
        if (existingUser is null)
            return NotFound("Пользователь не найден");

        // Обновляем только разрешенные поля
        // (не меняем пароль и ID)
        existingUser.Username = userDto.Username;
        existingUser.Email = userDto.Email;

        await _userAppService.UpdateUserAsync(existingUser);

        return Ok("Данные пользователя успешно обновлены");
    }

    /// <summary>
    /// Изменение аватарки пользователя
    /// </summary>
    /// <param name="file">Файл аватарки</param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("change-avatar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeAvatarAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не передан");

        if (file.Length > 5 * 1024 * 1024) // 5 MB
            return BadRequest("Слишком большой файл, не больше 5 МБ");

        string extension = Path.GetExtension(file.FileName);
        if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(extension))
            return BadRequest("Расширение должно быть jpg, jpeg или png");

        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        User? user = await _userAppService.GetUserByIdAsync(Guid.Parse(id));

        if (user is null)
            return NotFound("Пользователь не найден");

        try
        {
            using Stream stream = file.OpenReadStream();
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(id)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType));

            await _userAppService.UpdateUserAsync(user);

            return Ok("Аватарка успешно добавлена");
        }
        catch (Exception)
        {
            return BadRequest("Minio не запущен");
        }
        
    }

    /// <summary>
    /// Получение аватара
    /// </summary>
    /// <returns>Файл аватара</returns>
    [Authorize]
    [HttpGet("get-avatar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAvatarAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        try
        {
            ObjectStat stat = await _minioClient.StatObjectAsync(
            new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(id));

            MemoryStream memoryStream = new();
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(id)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream)));

            memoryStream.Position = 0;

            string contentType = stat.ContentType ?? "application/octet-stream";
            string fileName = contentType.Contains("/") ? contentType.Split("/")[1] : "avatar";

            return File(memoryStream, contentType, fileName);
        } 
        catch
        {
            return NotFound("У вас нет аватара или Minio не запущен");
        }
    }

    /// <summary>
    /// Удаляет аватар
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("delete-avatar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAvatarAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            ObjectStat stat = await _minioClient.StatObjectAsync(
            new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(id));

            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(id));

            return Ok("Аватар успешно удален");
        }
        catch
        {
            return NotFound("У вас нет аватара или Minio не запущен");
        }
    }

    // оставил для тестирования

    ///// <summary>
    ///// Удаляет бакет по названию
    ///// </summary>
    ///// <param name="bucketName">Название бакета</param>
    ///// <returns></returns>
    //[Authorize]
    //[HttpDelete("delete-bucket")]
    //public async Task<IActionResult> DeleteBucketAsync(string bucketName)
    //{
    //    bool isExists = await _minioClient.BucketExistsAsync(
    //            new BucketExistsArgs().WithBucket(bucketName));

    //    if (!isExists)
    //        return NotFound("Бакет не найден");

    //    await _minioClient.RemoveBucketAsync(
    //        new RemoveBucketArgs().WithBucket(bucketName));

    //    return Ok("Бакет удален");
    //}

    //[HttpGet("get-role")]
    //public async Task<IActionResult> GetRole()
    //{
    //    return Ok(User.FindFirstValue(ClaimTypes.Role));
    //}

    private async Task SignInWithClaimsAsync(UserRole role, string id)
    {
        string userRole = role switch
        {
            UserRole.Admin => "Admin",
            UserRole.Manager => "Manager",
            UserRole.DefaultUser => "DefaultUser",
            _ => throw new UnauthorizedAccessException()
        };

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, id),
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

    }
}

/// <summary>
/// Запрос на вход в аккаунт
/// </summary>
/// <param name="Email">Почта</param>
/// <param name="Password">Пароль</param>
public record SignInRequest(string Email, string Password);
