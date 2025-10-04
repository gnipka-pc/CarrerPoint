using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IUserAppService
{
    public Task<UserDto?> GetUserByIdAsync(Guid id);

    public Task<List<UserDto>> GetUsersAsync();

    public Task CreateUserAsync(UserDto userDto);

    public Task DeleteUserAsync(UserDto userDto);

    public Task UpdateUserAsync(UserDto userDto);

    //public Task<User> GetUserByIdAsync(Guid id);

    //public Task<List<User>> GetUsersAsync();

    //public Task CreateUserAsync(User user);

    //public Task DeleteUserAsync(Guid id);

    //public Task UpdateUserAsync(User user);
}
