using Microsoft.AspNetCore.SignalR;
using static Host.Infrastructure.Notifications.NotificationConstants;

namespace Host.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _context;    

    public NotificationService(IHubContext<NotificationHub> notificationHubContext) => (_context) = (notificationHubContext);

    public Task BroadcastAsync(INotificationMessage notification, CancellationToken cancellationToken)
    {
        return _context.Clients.All
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }
    public Task SendToGroupAsync(INotificationMessage notification, string group, CancellationToken cancellationToken)
    {
        return _context.Clients.Group(group)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
    }
}