namespace CareerPoint.Infrastructure.DTOs;

/// <summary>
/// Вариант ответа для поля типа Select / Radio / Checkbox
/// </summary>
public class QuestionOptionDto
{
    /// <summary>Отображаемый текст варианта</summary>
    public required string Text { get; set; }

    /// <summary>Внутренний ключ варианта (если не указан — используется Text)</summary>
    public string? Value { get; set; }
}
