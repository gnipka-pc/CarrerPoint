using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Application.Services;

public class UserAppService : IUserAppService
{
    readonly CareerPointContext _context;
    readonly DbSet<User> _users;
    readonly IPasswordHasher<User> _hasher;
    readonly IMapper _mapper;

    public UserAppService(
        CareerPointContext context,
        IPasswordHasher<User> hasher,
        IMapper mapper)
    {
        _context = context;
        _users = context.Users;
        _hasher = hasher;
        _mapper = mapper;
    }

    public async Task<UserDto> CreateUserAsync(User user)
    {
        user.HashedPassword = _hasher.HashPassword(user, user.HashedPassword);
        await _users.AddAsync(user);

        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUserAsync(User user)
    {
        _users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        User? user = await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        return user;
    }

    public async Task<List<User>> GetUsersAsync(UserFilterDto? filter = null)
    {
        IQueryable<User> query = _users.AsNoTracking();
        
        if (filter != null)
        {
            if (filter.Projects is { Count: > 0 })
                query = query.Where(u => filter.Projects.Contains(u.Project));

            if (filter.Directions is { Count: > 0 })
                query = query.Where(u => 
                    u.Directions.Any(d => filter.Directions.Contains(d))
                );

            if (filter.Courses is { Count: > 0 })
                query = query.Where(u => filter.Courses.Contains(u.Course));
        }
        
        return await query.ToListAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _users.Update(user);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> AddEventToUserAsync(Guid userId, Guid eventId)
    {
        User? user = await _users.Include(u => u.Events).FirstOrDefaultAsync(u => u.Id == userId);
        Event? ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);

        if (user is null || ev is null)
            return false;

        if (user.Events.Any(e => e.Id == eventId))
            return false;

        user.Events.Add(ev);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveEventFromUserAsync(Guid userId, Guid eventId)
    {
        User? user = await _users.Include(u => u.Events).FirstOrDefaultAsync(u => u.Id == userId);
        Event? ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);

        if (user is null || ev is null)
            return false;

        if (!user.Events.Any(e => e.Id == eventId))
            return false;

        user.Events.Remove(ev);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Event>> GetUserEventsAsync(Guid userId)
    {
        User? user = await _users.Include(u => u.Events).AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId) 
            ?? throw new ArgumentNullException("Пользователя с данным id не существует");

        return user.Events;
    }
}
