namespace CareerPoint.Infrastructure.DTOs;

public class FilledFieldDto
{
    public Guid FieldId { get; set; }

    /// <summary>Текст вопроса</summary>
    public required string Text { get; set; }

    public required string Value { get; set; }
}
