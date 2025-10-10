using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerPoint.Web.Controllers;

public class NotificationController : ControllerBase
{
    readonly INotificationAppService _notificationAppService;

    public NotificationController(
        INotificationAppService notificationAppService)
    {
        _notificationAppService = notificationAppService;
    }

    [Authorize(Roles = "Manager")]
    [HttpGet("get-subscribed-users")]
    public async Task<IActionResult> GetSubscribedUsersAsync()
    {
        List<UserDto> users = await _notificationAppService.GetSubscribedUsersAsync();

        if (users.Count == 0)
            return NotFound("Нет подписанных пользователей");

        return Ok(users);
    }

    [Authorize]
    [HttpPut("subscribe-to-notifications")]
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

    [Authorize]
    [HttpPut("unsubscribe-from-notifications")]
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
