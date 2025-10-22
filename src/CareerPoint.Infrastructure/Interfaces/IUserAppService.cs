using CareerPoint.Infrastructure.DTOs;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IUserAppService
{
    public Task<UserDto?> GetUserByIdAsync(Guid id);

    public Task<List<UserDto>> GetUsersAsync();

    public Task CreateUserAsync(UserDto userDto);

    public Task DeleteUserAsync(UserDto userDto);

    public Task UpdateUserAsync(UserDto userDto);

    public Task<bool> AddEventToUserAsync(Guid userId, Guid eventId);

    public Task<bool> RemoveEventFromUserAsync(Guid userId, Guid eventId);

    public Task<List<EventDto>> GetUserEventsAsync(Guid userId);
}
