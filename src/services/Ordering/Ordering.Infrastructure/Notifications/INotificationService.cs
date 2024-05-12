using Ordering.Host.Common.Interfaces;
using Ordering.Infrastructure.Notifications.Messages;

namespace Ordering.Infrastructure.Notifications
{
    public interface INotificationService : ITransientService
    {
        Task BroadcastAsync(INotificationMessage notification, CancellationToken cancellationToken);
        Task SendToGroupAsync(INotificationMessage notification, string group, CancellationToken cancellationToken);
    }
}
