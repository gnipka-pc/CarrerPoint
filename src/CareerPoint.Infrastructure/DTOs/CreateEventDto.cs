using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class CreateEventDto
{
    public required string Title { get; set; }

    public required string Description { get; set; }

    public EventType EventType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Organization { get; set; }

    public string[] HardSkills { get; set; } = Array.Empty<string>();

    public string? Position { get; set; }
}
