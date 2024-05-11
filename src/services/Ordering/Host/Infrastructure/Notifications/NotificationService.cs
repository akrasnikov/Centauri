using Microsoft.AspNetCore.SignalR;
using Ordering.Host.Infrastructure.Notifications.Messages;

namespace Ordering.Host.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _context;

    public NotificationService(IHubContext<NotificationHub> notificationHubContext) => _context = notificationHubContext;

    public Task BroadcastAsync(INotificationMessage notification, CancellationToken cancellationToken)
    {
        return
            _context
            .Clients
            .All
            .SendAsync(NotificationConstants.OrderNotification, notification.GetType().Name, notification, cancellationToken);

    }
    public Task SendToGroupAsync(INotificationMessage notification, string group, CancellationToken cancellationToken)
    {
        return
            _context
            .Clients
            .Group(group)
            .SendAsync(NotificationConstants.NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }
}