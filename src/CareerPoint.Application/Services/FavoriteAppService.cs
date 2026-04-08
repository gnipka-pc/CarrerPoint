using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Application.Services;

/// <summary>
/// Простой сервис работы с избранными ивентами.
/// Реализация максимально прямолинейная и понятная.
/// </summary>
public class FavoriteAppService : IFavoriteAppService
{
    private readonly CareerPointContext _context;
    private readonly DbSet<EventFavorite> _favorites;

    public FavoriteAppService(CareerPointContext context)
    {
        _context = context;
        _favorites = context.EventFavorites;
    }

    public async Task<List<Event>> GetFavoriteEventsAsync(Guid userId)
    {
        // Берём все записи избранного пользователя и подгружаем ивенты
        return await _favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Event)
            .AsNoTracking()
            .Select(f => f.Event)
            .ToListAsync();
    }

    public async Task<bool> AddFavoriteAsync(Guid userId, Guid eventId)
    {
        // Проверим, что пользователь и ивент существуют
        bool userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        bool eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);

        if (!userExists || !eventExists)
            return false;

        // Уже в избранном?
        bool alreadyExists = await _favorites.AnyAsync(f => f.UserId == userId && f.EventId == eventId);
        if (alreadyExists)
            return false;

        var favorite = new EventFavorite
        {
            UserId = userId,
            EventId = eventId,
            CreatedAt = DateTime.UtcNow
        };

        await _favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid eventId)
    {
        EventFavorite? favorite = await _favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

        if (favorite is null)
            return false;

        _favorites.Remove(favorite);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, Guid eventId)
    {
        return await _favorites.AnyAsync(f => f.UserId == userId && f.EventId == eventId);
    }

    public async Task<(bool success, bool isFavoriteNow)> ToggleFavoriteAsync(Guid userId, Guid eventId)
    {
        bool userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        bool eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);

        if (!userExists || !eventExists)
            return (false, false);

        EventFavorite? favorite = await _favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

        if (favorite is null)
        {
            await _favorites.AddAsync(new EventFavorite
            {
                UserId = userId,
                EventId = eventId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return (true, true);
        }

        _favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return (true, false);
    }
}
