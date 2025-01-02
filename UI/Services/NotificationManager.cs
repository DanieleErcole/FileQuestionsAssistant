using Avalonia.Controls.Notifications;
using UI.Views;

namespace UI.Services;

public class NotificationManager(MainWindow mw) : INotificationService {

    private readonly WindowNotificationManager _notificationManager = new(mw) {
        Position = NotificationPosition.BottomRight,
        MaxItems = 3,
    };
    
    public void ShowNotification(string title, string? message, NotificationType type = NotificationType.Information) =>
        _notificationManager.Show(new Notification {
            Title = title,
            Message = message,
            Type = type,
        });
    
}