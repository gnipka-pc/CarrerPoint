using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IUserAppService
{
    Task<User?> GetUserByIdAsync(Guid id);

    Task<List<User>> GetUsersAsync();

    /// <summary>Получить список пользователей с фильтрацией по проекту, направлению и курсу</summary>
    Task<List<User>> GetUsersFilteredAsync(UserFilterDto filter);

    Task CreateUserAsync(User userDto);

    Task DeleteUserAsync(User userDto);

    Task UpdateUserAsync(User userDto);

    Task<bool> AddEventToUserAsync(Guid userId, Guid eventId);

    Task<bool> RemoveEventFromUserAsync(Guid userId, Guid eventId);

    Task<List<Event>> GetUserEventsAsync(Guid userId);
}
