using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class FormFieldDto
{
    /// <summary>Текст вопроса</summary>
    public required string Text { get; set; }

    /// <summary>Дополнительное объяснение / подсказка к вопросу</summary>
    public string? Description { get; set; }

    public FieldType Type { get; set; }

    public bool IsRequired { get; set; }

    public int Order { get; set; }

    /// <summary>Варианты для Select / Radio / Checkbox</summary>
    public List<QuestionOptionDto>? Options { get; set; }
}
