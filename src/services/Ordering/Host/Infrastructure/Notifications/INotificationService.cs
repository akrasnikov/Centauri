using Ordering.Host.Common.Interfaces;
using Ordering.Host.Infrastructure.Notifications.Messages;

namespace Ordering.Host.Infrastructure.Notifications
{
    public interface INotificationService : ITransientService
    {
        Task BroadcastAsync(INotificationMessage notification, CancellationToken cancellationToken);
        Task SendToGroupAsync(INotificationMessage notification, string group, CancellationToken cancellationToken);
    }
}
