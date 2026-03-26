using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IUserAppService
{
    public Task<User?> GetUserByIdAsync(Guid id);

    public Task<List<User>> GetUsersAsync();

    public Task CreateUserAsync(User userDto);

    public Task DeleteUserAsync(User userDto);

    public Task UpdateUserAsync(User userDto);

    public Task<bool> AddEventToUserAsync(Guid userId, Guid eventId);

    public Task<bool> RemoveEventFromUserAsync(Guid userId, Guid eventId);

    public Task<List<Event>> GetUserEventsAsync(Guid userId);
}
