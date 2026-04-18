using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

/// <summary>
/// Параметры фильтрации списка пользователей.
/// Все поля опциональны и поддерживают мультивыбор.
/// </summary>
public class UserFilterDto
{
    /// <summary>Фильтр по проекту (Pazl / Code). Пусто — все проекты.</summary>
    public List<Project>? Projects { get; set; }

    /// <summary>Фильтр по направлению (Backend / Frontend / Design). Пусто — все направления.</summary>
    public List<Direction>? Directions { get; set; }

    /// <summary>Фильтр по курсу (1–4). Пусто — все курсы.</summary>
    public List<Course>? Courses { get; set; }
}
