namespace Ordering.Infrastructure.Notifications.Messages;

public class BasicNotification : INotificationMessage
{
    public enum LabelType
    {
        Information,
        Success,
        Warning,
        Error
    }

    public string? Message { get; set; }
    public LabelType Label { get; set; }
    public string Id { get; set; }

    public int Progress { get; set; }
}