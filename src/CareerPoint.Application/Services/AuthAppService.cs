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
    readonly IMapper _mapper;
    readonly CareerPointContext _context;
    readonly IPasswordHasher<UserDto> _hasher;

    public AuthAppService(IMapper mapper,
        CareerPointContext context,
        IPasswordHasher<UserDto> hasher)
    {
        _mapper = mapper;
        _context = context;
        _hasher = hasher;
    }

    public async Task<UserDto?> FindUserByEmailAndPasswordAsync(string email, string password)
    {
        User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            UserDto userDto = _mapper.Map<UserDto>(user);
            if (_hasher.VerifyHashedPassword(userDto, userDto.Password, password) == PasswordVerificationResult.Success)
            {
                Console.WriteLine("Все верифицировано успешно!");
                return userDto;
            }
        }

        return null;
    }
}
