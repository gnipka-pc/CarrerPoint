using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IUserAppService
{
    Task<User?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Получить список пользователей.
    /// Если filter передан — применяет фильтрацию по проекту, направлению и курсу.
    /// </summary>
    Task<List<User>> GetUsersAsync(UserFilterDto? filter = null);

    Task CreateUserAsync(User userDto);

    Task DeleteUserAsync(User userDto);

    Task UpdateUserAsync(User userDto);

    Task<bool> AddEventToUserAsync(Guid userId, Guid eventId);

    Task<bool> RemoveEventFromUserAsync(Guid userId, Guid eventId);

    Task<List<Event>> GetUserEventsAsync(Guid userId);
}
