using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerPoint.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    readonly IAuthAppService _authAppService;

    public AuthController(IAuthAppService authAppService)
    {
        _authAppService = authAppService;
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        UserDto? user = await _authAppService.FindUserByEmailAndPasswordAsync(request.Email, request.Password);

        if (user is null)
            return BadRequest("Почта или пароль не правильные");

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(2)
            });

        return Ok("Вы успешно вошли в аккаунт");
    }

    [HttpGet("get-claims")]
    [Authorize]
    public IActionResult GetClaims()
    {
        return Ok(User.Claims.Select(c => new {c.Type, c.Value}));
    }
}

public record SignInRequest(string Email, string Password);