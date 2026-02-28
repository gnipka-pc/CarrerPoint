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

    /// <summary>Уникальный ключ поля внутри формы (например: "firstName", "groupNumber")</summary>
    public required string Key { get; set; }

    /// <summary>Отображаемое название поля</summary>
    public required string Label { get; set; }

    public string? Placeholder { get; set; }

    public FieldType Type { get; set; }

    public bool IsRequired { get; set; } = false;

    /// <summary>Порядок отображения поля</summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Варианты ответов для Select / Radio / Checkbox-полей.
    /// Хранится как строка через запятую: "Вариант 1,Вариант 2"
    /// </summary>
    public string? Options { get; set; }
}
