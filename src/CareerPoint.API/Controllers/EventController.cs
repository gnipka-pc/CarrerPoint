using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerPoint.Web.Controllers;

/// <summary>
/// Контроллер ивентов
/// </summary>
[Route("api/events")]
[ApiController]
public class EventController : ControllerBase
{
    readonly IEventAppService _eventAppService;

    /// <summary>
    /// Базовый конструктор контроллера ивентов
    /// </summary>
    /// <param name="eventAppService">Апп сервис ивентов</param>
    public EventController(IEventAppService eventAppService)
    {
        _eventAppService = eventAppService;
    }

    /// <summary>
    /// Получение ивента по его айди
    /// </summary>
    /// <param name="id">Айди ивента</param>
    /// <returns>Ивент</returns>
    [HttpGet("get-event/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventByIdAsync(Guid id)
    {
        EventDto? ev = await _eventAppService.GetEventByIdAsync(id);

        if (ev != null)
            return Ok(ev);

        return NotFound("Ивент не найден");
    }

    /// <summary>
    /// Получение списка ивентов
    /// </summary>
    /// <returns>Список ивентов</returns>
    [HttpGet("get-events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventsAsync()
    {
        return Ok(await _eventAppService.GetEventsAsync());
    }

    /// <summary>
    /// Создание ивента
    /// </summary>
    /// <param name="ev">Ивент</param>
    /// <returns></returns>
    [Authorize(Roles = "Manager")]
    [HttpPost("create-event")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateEventAsync([FromBody] EventDto ev)
    {
        if (await _eventAppService.GetEventByIdAsync(ev.Id) == null)
        {
            await _eventAppService.CreateEventAsync(ev);

            return Ok(ev);
        }

        return BadRequest("Ивент с данным Id уже существует");
    }

    /// <summary>
    /// Удаление ивента по его айди
    /// </summary>
    /// <param name="id">Айди ивента</param>
    /// <returns></returns>
    [Authorize(Roles = "Manager")]
    [HttpDelete("delete-event/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteEventAsync(Guid id)
    {
        EventDto? ev = await _eventAppService.GetEventByIdAsync(id);
        
        if (ev != null)
        {
            await _eventAppService.DeleteEventAsync(ev);
            return Ok("Ивент удален успешно");
        }

        return NotFound("Ивент с данным id не был найден");
    }

    /// <summary>
    /// Обновление ивента 
    /// </summary>
    /// <param name="newEvent">Ивент</param>
    /// <returns></returns>
    [Authorize(Roles = "Manager")]
    [HttpPut("update-event")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEventAsync([FromBody] EventDto newEvent)
    {
        if (await _eventAppService.GetEventByIdAsync(newEvent.Id) != null)
        {
            await _eventAppService.UpdateEventAsync(newEvent);
            return Ok("Ивент изменен успешно");
        }
            
        return NotFound("Ивент с данным id не был найден");
    }
}
