using Avalonia.Controls.Notifications;

namespace UI.Services;

public interface INotificationService {
    void ShowNotification(string title, string? message, NotificationType type = NotificationType.Information);
}