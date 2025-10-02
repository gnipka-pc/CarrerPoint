using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.Model;

public class Event
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public EventType EventType { get; set; }

    public List<User> Users { get; set; } = new();
}
