using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.Model;

public class Event
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public EventType EventType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Organization { get; set; }

    public string[] HardSkills { get; set; } = Array.Empty<string>();

    public string? Position { get; set; }

    public List<User> Users { get; set; } = new();

    public List<EventFavorite> EventFavorites { get; set; } = new();
}
