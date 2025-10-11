using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace CareerPoint.Application.Services;

public class NotificationAppService : INotificationAppService
{
    readonly CareerPointContext _context;
    readonly DbSet<User> _users;
    readonly IMapper _mapper;

    public NotificationAppService(
        CareerPointContext context,
        IMapper mapper)
    {
        _context = context;
        _users = context.Users;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetSubscribedUsersAsync()
    {
        return await _users.AsNoTracking().Where(u => u.IsSubscribedToNotifications == true).Select(u => _mapper.Map<UserDto>(u)).ToListAsync();
    }

    public async Task<bool> SubscribeToNotificationsAsync(Guid userId)
    {
        User? user = await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.IsSubscribedToNotifications == true)
        {
            return false;
        }

        user.IsSubscribedToNotifications = true;

        _users.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnsubscribeFromNotificationsAsync(Guid userId)
    {
        User? user = await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.IsSubscribedToNotifications == false)
        {
            return false;
        }

        user.IsSubscribedToNotifications = false;

        _users.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
