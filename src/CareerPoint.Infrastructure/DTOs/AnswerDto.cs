using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class AnswerDto
{
    public Guid FieldId { get; set; }

    public required string Value { get; set; }
}