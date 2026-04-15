using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IEventAppService
{
    public Task<EventDto?> GetEventByIdAsync(Guid id);

    public Task<List<EventDto>> GetEventsAsync();

    public Task<EventDto> CreateEventAsync(CreateEventDto createDto);

    public Task DeleteEventAsync(Guid id);

    public Task UpdateEventAsync(Guid id, EventDto eventDto);
}
