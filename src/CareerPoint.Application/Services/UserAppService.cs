using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Application.Services;

public class UserAppService : IUserAppService
{
    readonly CareerPointContext _context;
    readonly DbSet<User> _users;
    readonly IMapper _mapper;

    public UserAppService(
        CareerPointContext context,
        IMapper mapper)
    {
        _context = context;
        _users = context.Users;
        _mapper = mapper;
    }

    public async Task CreateUserAsync(UserDto userDto)
    {
        await _users.AddAsync(_mapper.Map<User>(userDto));

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        User user = await _users.FindAsync(id) ?? throw new NullReferenceException("Пользователь не был найден");

        _users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        User user = await _users.FindAsync(id) ?? throw new NullReferenceException("Пользователь не был найден");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _users.Select(u => _mapper.Map<UserDto>(u)).ToListAsync();
    }

    public async Task UpdateUserAsync(UserDto userDto)
    {
        _users.Update(_mapper.Map<User>(userDto));

        int isChanged = await _context.SaveChangesAsync();

        if (isChanged == 0)
            throw new NullReferenceException("Событие не было найдено");
    }
}
