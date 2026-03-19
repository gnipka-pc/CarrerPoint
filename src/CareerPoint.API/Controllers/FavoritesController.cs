using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerPoint.Web.Controllers;

/// <summary>
/// Контроллер избранного (Favorite events).
/// </summary>
[Route("api/favorites")]
[ApiController]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteAppService _favoriteAppService;
    private readonly IMapper _mapper;

    public FavoritesController(IFavoriteAppService favoriteAppService, IMapper mapper)
    {
        _favoriteAppService = favoriteAppService;
        _mapper = mapper;
    }

    /// <summary>
    /// Получить избранные ивенты текущего пользователя
    /// </summary>
    [Authorize]
    [HttpGet("get-favorites")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFavoritesAsync()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        List<Event> events = await _favoriteAppService.GetFavoriteEventsAsync(Guid.Parse(id));
        if (events.Count == 0)
            return NotFound("Избранное пустое");

        return Ok(_mapper.Map<List<EventDto>>(events));
    }

    /// <summary>
    /// Добавить ивент в избранное
    /// </summary>
    [Authorize]
    [HttpPost("add/{eventId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToFavoritesAsync(Guid eventId)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        bool success = await _favoriteAppService.AddFavoriteAsync(Guid.Parse(id), eventId);
        if (!success)
            return BadRequest("Не удалось добавить (возможно уже добавлено или ивент/пользователь не найден)");

        return Ok("Добавлено в избранное");
    }

    /// <summary>
    /// Удалить ивент из избранного
    /// </summary>
    [Authorize]
    [HttpDelete("remove/{eventId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromFavoritesAsync(Guid eventId)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        bool success = await _favoriteAppService.RemoveFavoriteAsync(Guid.Parse(id), eventId);
        if (!success)
            return NotFound("Запись в избранном не найдена");

        return Ok("Удалено из избранного");
    }

    /// <summary>
    /// Toggle: если ивент не в избранном - добавит, если уже был - удалит.
    /// </summary>
    [Authorize]
    [HttpPost("toggle/{eventId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleFavoriteAsync(Guid eventId)
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null)
            return Unauthorized("Пользователь не авторизован");

        var result = await _favoriteAppService.ToggleFavoriteAsync(Guid.Parse(id), eventId);
        if (!result.success)
            return BadRequest("Пользователь или ивент не найден");

        // Возвращаем итоговое состояние, удобно фронту
        return Ok(new { isFavorite = result.isFavoriteNow });
    }
}
