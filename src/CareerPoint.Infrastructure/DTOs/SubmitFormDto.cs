using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class SubmitFormDto
{
    public List<AnswerDto> Answers { get; set; } = new();
}