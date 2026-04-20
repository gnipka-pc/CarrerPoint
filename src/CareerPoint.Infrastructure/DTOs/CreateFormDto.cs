using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class CreateFormDto
{
    public Guid EventId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    /// <summary>Дедлайн приёма заявок</summary>
    public DateTime? DeadlineAt { get; set; }

    public List<FormFieldDto> Fields { get; set; } = new();
}
