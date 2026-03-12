namespace CareerPoint.Infrastructure.Model;

/// <summary>
/// Вариант ответа для поля формы (Select / Radio / Checkbox)
/// </summary>
public class QuestionOption
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public FormField? Question { get; set; }

    public required string Text { get; set; }

    public int OrderIndex { get; set; }
}

