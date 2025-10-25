using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerPoint.Web.Controllers;

/// <summary>
/// Контроллер уведомлений
/// </summary>
public class NotificationController : ControllerBase
{
    readonly INotificationAppService _notificationAppService;

    /// <summary>
    /// Базовый конструктор контроллера уведомлений
    /// </summary>
    /// <param name="notificationAppService">Апп сервис уведомлений</param>
    public NotificationController(
        INotificationAppService notificationAppService)
    {
        _notificationAppService = notificationAppService;
    }

    /// <summary>
    /// Получение списка подписанных на уведомления пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    [Authorize(Roles = "Manager")]
    [HttpGet("get-subscribed-users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSubscribedUsersAsync()
    {
        List<UserDto> users = await _notificationAppService.GetSubscribedUsersAsync();

        if (users.Count == 0)
            return NotFound("Нет подписанных пользователей");

        return Ok(users);
    }

    /// <summary>
    /// Подписка пользователя на уведомления
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPut("subscribe-to-notifications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SubscribeToNotificationsAsync()
    {
        bool isParsed = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);

        if (!isParsed)
            return BadRequest("Некорректный идентификатор пользователя");

        bool isSucess = await _notificationAppService.SubscribeToNotificationsAsync(userId);

        if (isSucess)
            return Ok("Пользователь успешно подписался на уведомления");
        
        return BadRequest("Пользователь уже подписан на уведомления");
    }

    /// <summary>
    /// Отписка пользователя от уведомлений
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPut("unsubscribe-from-notifications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnsubscribeFromNotificationsAsync()
    {
        bool isParsed = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);

        if (!isParsed)
            return BadRequest("Некорректный идентификатор пользователя");

        bool isSucess = await _notificationAppService.UnsubscribeFromNotificationsAsync(userId);
        if (isSucess)
            return Ok("Пользователь успешно отписался от уведомлений");

        return BadRequest("Пользователь уже отписан от уведомлений");
    }
}
