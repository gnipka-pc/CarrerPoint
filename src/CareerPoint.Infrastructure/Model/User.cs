using CareerPoint.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;

namespace CareerPoint.Infrastructure.Model;

public class User
{
    public Guid Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string HashedPassword { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public string? Patronymic { get; set; }

    public string Description { get; set; } = "Empty description";

    public required string TelegramLink { get; set; }

    public required string PortfolioLink { get; set; }

    public bool IsSubscribedToNotifications { get; set; }

    // Возраст пользователя
    public int Age { get; set; }

    // Направление обучения
    public string Direction { get; set; } = string.Empty;

    // Курс (1-6)
    public int Course { get; set; }

    // Навыки через запятую
    public string Skills { get; set; } = string.Empty;

    public UserRole UserRole { get; set; }

    public List<Event> Events { get; set; } = new();
}
