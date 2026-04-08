namespace CareerPoint.Infrastructure.Model;

/// <summary>
/// Избранное: связующая сущность User <-> Event.
/// </summary>
public class EventFavorite
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    /// <summary>
    /// Когда добавили в избранное.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
