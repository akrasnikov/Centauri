using Host.Common.Interfaces;

namespace Host.Infrastructure.Notifications
{
    public interface INotificationService : ITransientService
    {
        Task BroadcastAsync(INotificationMessage notification, CancellationToken cancellationToken);          
        Task SendToGroupAsync(INotificationMessage notification, string group, CancellationToken cancellationToken);             
    }
}
