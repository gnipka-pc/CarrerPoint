using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.Model;

/// <summary>
/// Поле формы регистрации
/// </summary>
public class FormField
{
    public Guid Id { get; set; }

    public Guid FormId { get; set; }

    public Form? Form { get; set; }

    /// <summary>Текст вопроса</summary>
    public required string Text { get; set; }

    /// <summary>Дополнительное объяснение к вопросу</summary>
    public string? Description { get; set; }

    public FieldType Type { get; set; }

    public bool IsRequired { get; set; } = false;

    /// <summary>Порядок отображения поля</summary>
    public int Order { get; set; } = 0;

    /// <summary>Варианты ответов для Select / Radio / Checkbox-полей</summary>
    public List<QuestionOption> Options { get; set; } = new();
}
