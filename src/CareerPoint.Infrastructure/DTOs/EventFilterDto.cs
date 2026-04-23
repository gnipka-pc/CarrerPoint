using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

/// <summary>
/// Параметры фильтрации событий.
/// Все поля необязательные.
/// </summary>
public class EventFilterDto
{
    /// <summary>
    /// Список типов событий. Если пусто — все типы.
    /// </summary>
    public List<EventType>? EventTypes { get; set; }

    /// <summary>
    /// Дата начала события, от которой фильтруем.
    /// </summary>
    public DateTime? StartDateFrom { get; set; }

    /// <summary>
    /// Дата начала события, до которой фильтруем.
    /// </summary>
    public DateTime? StartDateTo { get; set; }

    /// <summary>
    /// Дата завершения события, от которой фильтруем.
    /// </summary>
    public DateTime? EndDateFrom { get; set; }

    /// <summary>
    /// Дата завершения события, до которой фильтруем.
    /// </summary>
    public DateTime? EndDateTo { get; set; }
}
