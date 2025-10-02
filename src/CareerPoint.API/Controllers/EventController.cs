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
        return Ok(await _eventAppService.GetEventByIdAsync(id));
    }

    [HttpGet("get-events")]
    public async Task<IActionResult> GetEventsAsync()
    {
        return Ok(await _eventAppService.GetEventsAsync());
    }

    [HttpPost("create-event")]
    public async Task<IActionResult> CreateEventAsync([FromBody] EventDto ev)
    {
        await _eventAppService.CreateEventAsync(ev);

        return Ok("Event создан успешно");
    }

    [HttpDelete("delete-event/{id}")]
    public async Task<IActionResult> DeleteEventAsync(Guid id)
    {
        await _eventAppService.DeleteEventAsync(id);

        return Ok("Event удален успешно");
    }

    [HttpPut("update-event")]
    public async Task<IActionResult> UpdateEventAsync([FromBody] EventDto newEvent)
    {
        await _eventAppService.UpdateEventAsync(newEvent);

        return Ok("Event изменен успешно");
    }
}
