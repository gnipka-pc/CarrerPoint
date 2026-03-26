using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Infrastructure.Interfaces;

/// <summary>
/// Сервис работы с избранным (Favorites).
/// </summary>
public interface IFavoriteAppService
{
    /// <summary>
    /// Получить список избранных ивентов пользователя.
    /// </summary>
    Task<List<Event>> GetFavoriteEventsAsync(Guid userId);

    /// <summary>
    /// Добавить ивент в избранное. Вернёт false, если уже был в избранном или если user/event не найдены.
    /// </summary>
    Task<bool> AddFavoriteAsync(Guid userId, Guid eventId);

    /// <summary>
    /// Удалить ивент из избранного. Вернёт false, если записи не было.
    /// </summary>
    Task<bool> RemoveFavoriteAsync(Guid userId, Guid eventId);

    /// <summary>
    /// Проверить, находится ли ивент в избранном.
    /// </summary>
    Task<bool> IsFavoriteAsync(Guid userId, Guid eventId);

    /// <summary>
    /// Toggle: если не было в избранном - добавляет, если было - удаляет.
    /// success=false, если user/event не найдены.
    /// isFavoriteNow - итоговое состояние.
    /// </summary>
    Task<(bool success, bool isFavoriteNow)> ToggleFavoriteAsync(Guid userId, Guid eventId);
}
