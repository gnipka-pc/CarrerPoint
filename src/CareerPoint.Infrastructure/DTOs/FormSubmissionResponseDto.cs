using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;
public class FormSubmissionResponseDto
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<AnswerResponseDto> Answers { get; set; } = new();
}