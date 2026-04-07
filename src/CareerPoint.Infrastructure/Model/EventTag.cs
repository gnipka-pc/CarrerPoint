namespace CareerPoint.Infrastructure.Model;

public class EventTag
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public required string Key { get; set; }

    public required string Value { get; set; }

    public Event Event { get; set; } = null!;
}
