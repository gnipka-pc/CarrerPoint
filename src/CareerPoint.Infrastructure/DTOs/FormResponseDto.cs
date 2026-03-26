using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;
public class FormResponseDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<FormFieldResponseDto> Fields { get; set; } = new();
}