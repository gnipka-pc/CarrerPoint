using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;
/// <summary>Форма с заполненными ответами для студента</summary>
public class FormWithMyAnswersDto
{
    public Guid FormId { get; set; }
    public Guid EventId { get; set; }
    public required string FormTitle { get; set; }
    public Guid SubmissionId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<FilledFieldDto> Fields { get; set; } = new();
}