namespace CareerPoint.Infrastructure.Model;

/// <summary>
/// Форма регистрации, привязанная к мероприятию
/// </summary>
public class Form
{
    public Guid Id { get; set; }

    /// <summary>Ссылка на мероприятие, для которого создана форма</summary>
    public Guid EventId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    /// <summary>Открыта ли форма для приёма ответов студентов</summary>
    public bool IsOpen { get; set; } = true;

    /// <summary>Дедлайн приёма заявок (опционально)</summary>
    public DateTime? DeadlineAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<FormField> Fields { get; set; } = new();

    public List<FormSubmission> Submissions { get; set; } = new();

    public Event? Event { get; set; }
}
