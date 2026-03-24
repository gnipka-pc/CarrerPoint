using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Application.Services;

public class AuthAppService : IAuthAppService
{
    readonly CareerPointContext _context;
    readonly IPasswordHasher<User> _hasher;

    public AuthAppService(
        CareerPointContext context,
        IPasswordHasher<User> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public async Task<User?> FindUserByEmailAndPasswordAsync(string email, string password)
    {
        User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            if (_hasher.VerifyHashedPassword(user, user.HashedPassword, password) == PasswordVerificationResult.Success)
            {
                Console.WriteLine("Все верифицировано успешно!");
                return user;
            }
        }

        return null;
    }
}
