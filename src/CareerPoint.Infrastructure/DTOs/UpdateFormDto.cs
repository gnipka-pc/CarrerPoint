using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

public class UpdateFormDto
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public List<FormFieldDto> Fields { get; set; } = new();
}
