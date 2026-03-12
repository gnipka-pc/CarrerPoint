using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class FormFieldResponseDto
{
    public Guid Id { get; set; }

    /// <summary>Текст вопроса</summary>
    public required string Text { get; set; }

    /// <summary>Дополнительное объяснение к вопросу</summary>
    public string? Description { get; set; }

    public FieldType Type { get; set; }

    public bool IsRequired { get; set; }

    public int Order { get; set; }

    public List<string> Options { get; set; } = new();
}
