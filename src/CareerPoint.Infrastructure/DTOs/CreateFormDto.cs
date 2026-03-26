using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

//Запросы (входящие данные)
public class CreateFormDto
{
    public Guid EventId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public List<FormFieldDto> Fields { get; set; } = new();
}
