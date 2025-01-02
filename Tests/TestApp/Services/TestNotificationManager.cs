using Avalonia.Controls.Notifications;
using UI.Services;

namespace Tests.TestApp.Services;

public class TestNotificationManager : INotificationService {
    public void ShowNotification(string title, string? message, NotificationType type = NotificationType.Information) 
        => Console.WriteLine($@"Notification: {title} - {message}");
}