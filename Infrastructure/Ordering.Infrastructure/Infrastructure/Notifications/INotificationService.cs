using Host.Common.Interfaces;
using Ordering.Infrastructure.Infrastructure.Notifications.Messages;

namespace Ordering.Infrastructure.Infrastructure.Notifications
{
    public interface INotificationService : ITransientService
    {
        Task BroadcastAsync(INotificationMessage notification, CancellationToken cancellationToken);
        Task SendToGroupAsync(INotificationMessage notification, string group, CancellationToken cancellationToken);
    }
}
