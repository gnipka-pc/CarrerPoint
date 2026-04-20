namespace CareerPoint.Infrastructure.Model;

/// <summary>
/// Вариант ответа для поля формы (Select / Radio / Checkbox)
/// </summary>
public class QuestionOption
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public FormField? Question { get; set; }

    /// <summary>Отображаемый текст варианта</summary>
    public required string Text { get; set; }

    /// <summary>Внутренний ключ варианта (используется при сохранении ответа)</summary>
    public string? Value { get; set; }

    public int OrderIndex { get; set; }
}
