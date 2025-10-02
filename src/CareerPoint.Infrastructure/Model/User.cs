using Microsoft.AspNetCore.Identity;

namespace CareerPoint.Infrastructure.Model;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public required string Surname { get; set; }

    public required string Patronymic { get; set; }

    public string Description { get; set; } = "Empty description";

    public required string TelegramLink { get; set; }

    public required string PortfolioLink { get; set; }

    public List<Event> Events { get; set; } = new();
}
