using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class UpdateFormDto
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    /// <summary>Открыта ли форма для приёма ответов</summary>
    public bool IsOpen { get; set; }

    /// <summary>Дедлайн приёма заявок</summary>
    public DateTime? DeadlineAt { get; set; }

    public List<FormFieldDto> Fields { get; set; } = new();
}
