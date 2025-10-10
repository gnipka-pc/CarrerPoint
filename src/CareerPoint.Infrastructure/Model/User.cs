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

    public bool IsSubscribedToEvents { get; set; }

    public UserRole UserRole { get; set; }

    public List<Event> Events { get; set; } = new();
}
