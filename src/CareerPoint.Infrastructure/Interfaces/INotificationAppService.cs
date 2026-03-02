using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Infrastructure.Interfaces;

public interface INotificationAppService
{
    public Task<List<User>> GetSubscribedUsersAsync();

    public Task<bool> SubscribeToNotificationsAsync(Guid userId);

    public Task<bool> UnsubscribeFromNotificationsAsync(Guid userId);
}
