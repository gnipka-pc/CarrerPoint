using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CareerPoint.Application.Services;

public class UserAppService : IUserAppService
{
    readonly CareerPointContext _context;
    readonly DbSet<User> _users;
    readonly IMapper _mapper;
    readonly IPasswordHasher<UserDto> _hasher;

    public UserAppService(
        CareerPointContext context,
        IMapper mapper,
        IPasswordHasher<UserDto> hasher)
    {
        _context = context;
        _users = context.Users;
        _mapper = mapper;
        _hasher = hasher;
    }

    public async Task CreateUserAsync(UserDto userDto)
    {
        userDto.Password = _hasher.HashPassword(userDto, userDto.Password);
        await _users.AddAsync(_mapper.Map<User>(userDto));

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(UserDto user)
    {
        _users.Remove(_mapper.Map<User>(user));
        await _context.SaveChangesAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        User? user = await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        return _mapper.Map<UserDto?>(user);
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _users.AsNoTracking().Select(u => _mapper.Map<UserDto>(u)).ToListAsync();
    }

    public async Task UpdateUserAsync(UserDto userDto)
    {
        _users.Update(_mapper.Map<User>(userDto));

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

        Console.WriteLine(user.Events.Count);
        Console.WriteLine(user.Events.Any(e => e.Id == eventId));
        Console.WriteLine();

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

    public async Task<List<EventDto>> GetUserEventsAsync(Guid userId)
    {
        User? user = await _users.Include(u => u.Events).AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId) 
            ?? throw new ArgumentNullException("Пользователя с данным id не существует");

        return _mapper.Map<List<EventDto>>(user.Events);
    }
}
