using Avalonia.Controls.Notifications;

namespace UI.Services;

public class NotificationManager(WindowNotificationManager notificationManager) : INotificationService {
    public void ShowNotification(string title, string? message, NotificationType type = NotificationType.Information) =>
        notificationManager.Show(new Notification {
            Title = title,
            Message = message,
            Type = type,
        });
}