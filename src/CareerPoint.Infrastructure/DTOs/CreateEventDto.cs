using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class CreateEventDto
{
    public required string Title { get; set; }

    public required string Description { get; set; }

    public EventType EventType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Dictionary<string, string> Tags { get; set; } = new();
}
