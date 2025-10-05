using CareerPoint.Infrastructure.DTOs;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IAuthAppService
{
    public Task<UserDto?> FindUserByEmailAndPasswordAsync(string email, string password);
}
