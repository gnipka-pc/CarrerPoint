using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class RegisterUserDto
{
    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public string? Patronymic { get; set; }

    public string Description { get; set; } = "Empty description";

    public required string TelegramLink { get; set; }

    public required string PortfolioLink { get; set; }

    public bool IsSubscribedToNotifications { get; set; }

    public int Age { get; set; }

    public Direction Direction { get; set; }

    /// <summary>Проект пользователя (ПАЗЛ / КОД)</summary>
    public Project Project { get; set; }

    public Course Course { get; set; }

    public string[] Skills { get; set; } = Array.Empty<string>();
}
