using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CareerPoint.Web.Controllers;

[Route("api/events")]
public class EventController : ControllerBase
{
    readonly IEventAppService _eventAppService;
    public EventController(IEventAppService eventAppService)
    {
        _eventAppService = eventAppService;
    }

    [HttpGet("get-event/{id}")]
    public async Task<IActionResult> GetEventByIdAsync(Guid id)
    {
        EventDto? ev = await _eventAppService.GetEventByIdAsync(id);

        if (ev != null)
            return Ok(ev);

        return NotFound("Ивент не найден");
    }

    [HttpGet("get-events")]
    public async Task<IActionResult> GetEventsAsync()
    {
        return Ok(await _eventAppService.GetEventsAsync());
    }

    [HttpPost("create-event")]
    public async Task<IActionResult> CreateEventAsync([FromBody] EventDto ev)
    {
        if (await _eventAppService.GetEventByIdAsync(ev.Id) == null)
        {
            await _eventAppService.CreateEventAsync(ev);

            return Ok(ev);
        }

        return BadRequest("Ивент с данным Id уже существует");
    }

    [HttpDelete("delete-event/{id}")]
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

    [HttpPut("update-event")]
    public async Task<IActionResult> UpdateEventAsync([FromBody] EventDto newEvent)
    {
        EventDto? ev = await _eventAppService.GetEventByIdAsync(newEvent.Id);

        if (ev != null)
        {
            await _eventAppService.UpdateEventAsync(ev);
            return Ok("Ивент изменен успешно");
        }
            
        return NotFound("Ивент с данным id не был найден");
    }
}
