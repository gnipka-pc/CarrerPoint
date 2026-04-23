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

    public async Task<EventDto> CreateEventAsync(CreateEventDto createDto)
    {
        var ev = _mapper.Map<Event>(createDto);
        ev.Id = Guid.NewGuid();

        await _events.AddAsync(ev);
        await _context.SaveChangesAsync();

        return _mapper.Map<EventDto>(ev);
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
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return _mapper.Map<EventDto?>(ev);
    }

    public async Task<List<EventDto>> GetEventsAsync(EventFilterDto? filterDto = null)
    {
        IQueryable<Event> query = _events.AsNoTracking();

        if (filterDto?.EventTypes is { Count: > 0 })
        {
            query = query.Where(ev => filterDto.EventTypes!.Contains(ev.EventType));
        }

        if (filterDto?.StartDateFrom != null)
        {
            query = query.Where(ev => ev.StartDate >= filterDto.StartDateFrom.Value);
        }

        if (filterDto?.StartDateTo != null)
        {
            query = query.Where(ev => ev.StartDate <= filterDto.StartDateTo.Value);
        }

        if (filterDto?.EndDateFrom != null)
        {
            query = query.Where(ev => ev.EndDate != null && ev.EndDate >= filterDto.EndDateFrom.Value);
        }

        if (filterDto?.EndDateTo != null)
        {
            query = query.Where(ev => ev.EndDate != null && ev.EndDate <= filterDto.EndDateTo.Value);
        }

        List<Event> events = await query
            .OrderByDescending(ev => ev.StartDate)
            .ToListAsync();

        return _mapper.Map<List<EventDto>>(events);
    }

    public async Task UpdateEventAsync(Guid id, EventDto eventDto)
    {
        var existingEvent = await _events.FirstOrDefaultAsync(e => e.Id == id);

        if (existingEvent == null)
            return;

        existingEvent.Title = eventDto.Title;
        existingEvent.Description = eventDto.Description;
        existingEvent.EventType = eventDto.EventType;
        existingEvent.StartDate = eventDto.StartDate;
        existingEvent.EndDate = eventDto.EndDate;
        existingEvent.Organization = eventDto.Organization;
        existingEvent.HardSkills = eventDto.HardSkills;
        existingEvent.Position = eventDto.Position;

        await _context.SaveChangesAsync();
    }
}
