using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;
public class FormResultsDto
{
    public Guid FormId { get; set; }
    public required string FormTitle { get; set; }
    public int TotalSubmissions { get; set; }
    public List<FormSubmissionResponseDto> Submissions { get; set; } = new();
}