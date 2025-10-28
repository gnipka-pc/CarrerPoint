using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IAuthAppService
{
    public Task<User?> FindUserByEmailAndPasswordAsync(string email, string password);
}
