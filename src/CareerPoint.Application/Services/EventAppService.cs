using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Application.Services;

public class EventAppService : IEventAppService
{
    readonly CareerPointContext _context;
    readonly DbSet<Event> _events;
    readonly IMapper _mapper;

    public EventAppService(
        CareerPointContext context,
        IMapper mapper)
    {
        _context = context;
        _events = context.Events;
        _mapper = mapper;
    }

    public async Task CreateEventAsync(CreateEventDto createDto)
    {
        var ev = _mapper.Map<Event>(createDto);
        ev.Id = Guid.NewGuid();

        await _events.AddAsync(ev);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(Guid id)
    {
        var ev = await _events.FindAsync(id);
        if (ev != null)
        {
            _events.Remove(ev);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id)
    {
        Event? ev = await _events
            .Include(e => e.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return _mapper.Map<EventDto?>(ev);
    }

    public async Task<List<EventDto>> GetEventsAsync()
    {
        return await _events
            .Include(e => e.Tags)
            .AsNoTracking()
            .Select(ev => _mapper.Map<EventDto>(ev))
            .ToListAsync();
    }

    public async Task UpdateEventAsync(Guid id, EventDto eventDto)
    {
        var existingEvent = await _events
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existingEvent == null)
            return;

        existingEvent.Title = eventDto.Title;
        existingEvent.Description = eventDto.Description;
        existingEvent.EventType = eventDto.EventType;
        existingEvent.StartDate = eventDto.StartDate;
        existingEvent.EndDate = eventDto.EndDate;

        // Update tags
        existingEvent.Tags.Clear();
        foreach (var tag in eventDto.Tags)
        {
            existingEvent.Tags.Add(new EventTag
            {
                Key = tag.Key,
                Value = tag.Value
            });
        }

        await _context.SaveChangesAsync();
    }
}
