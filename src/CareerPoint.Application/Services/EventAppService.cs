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

    public async Task CreateEventAsync(EventDto evDto)
    {
        await _events.AddAsync(_mapper.Map<Event>(evDto));

        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(EventDto ev)
    {
        _events.Remove(_mapper.Map<Event>(ev));
        await _context.SaveChangesAsync();
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id)
    {
        Event? ev = await _events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        return _mapper.Map<EventDto?>(ev);
    }

    public async Task<List<EventDto>> GetEventsAsync()
    {
        return await _events.AsNoTracking().Select(ev => _mapper.Map<EventDto>(ev)).ToListAsync();
    }

    public async Task UpdateEventAsync(EventDto newEventDto)
    {
        _events.Update(_mapper.Map<Event>(newEventDto));

        await _context.SaveChangesAsync();
    }
}
