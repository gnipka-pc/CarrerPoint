using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class UserDto
{
    public Guid Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public string? Patronymic { get; set; }

    public string Description { get; set; } = "Empty description";

    public required string TelegramLink { get; set; }

    public required string PortfolioLink { get; set; }

    public bool IsSubscribedToNotifications { get; set; }

    // Возраст
    public int Age { get; set; }

    // Направление
    public Direction Direction { get; set; }

    // Курс
    public Course Course { get; set; }

    // Навыки
    public string[] Skills { get; set; } = Array.Empty<string>();

    public UserRole UserRole { get; set; }

    public string? AvatarUrl { get; set; }
}
