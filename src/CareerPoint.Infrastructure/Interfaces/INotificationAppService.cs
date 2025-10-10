using CareerPoint.Infrastructure.DTOs;

namespace CareerPoint.Infrastructure.Interfaces;

public interface INotificationAppService
{
    public Task<List<UserDto>> GetSubscribedUsersAsync();

    public Task<bool> SubscribeToNotificationsAsync(Guid userId);

    public Task<bool> UnsubscribeFromNotificationsAsync(Guid userId);
}
