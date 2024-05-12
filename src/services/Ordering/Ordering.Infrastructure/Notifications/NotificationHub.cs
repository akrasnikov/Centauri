using Microsoft.AspNetCore.SignalR;
using Ordering.Host.Common.Interfaces;

namespace Ordering.Infrastructure.Notifications;

public class NotificationHub : Hub, ITransientService
{
    private readonly ILogger<NotificationHub> _logger;
    private const string _groupName = "orders";

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"group-{_groupName}");

        await base.OnConnectedAsync();

        _logger.LogInformation("A client connected to NotificationHub: {connectionId}", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group-{_groupName}");

        await base.OnDisconnectedAsync(exception);

        _logger.LogInformation("A client disconnected from NotificationHub: {connectionId}", Context.ConnectionId);
    }
}