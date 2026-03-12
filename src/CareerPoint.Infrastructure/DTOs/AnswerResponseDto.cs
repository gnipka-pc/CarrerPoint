namespace CareerPoint.Infrastructure.DTOs;

public class AnswerResponseDto
{
    public Guid FieldId { get; set; }

    /// <summary>Текст вопроса, на который дан ответ</summary>
    public required string FieldText { get; set; }

    public required string Value { get; set; }
}
