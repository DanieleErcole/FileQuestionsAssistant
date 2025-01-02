using System;
using Avalonia.Controls.Notifications;
using Serilog;
using UI.Utils;

namespace UI.Services;

public class ErrorHandler(INotificationService notificationManager) : IErrorHandlerService {
    public void ShowError(Exception ex) {
        Log.Error(ex, "Handled error");
        var err = ex as UIException ?? UIException.FromException(ex);
        notificationManager.ShowNotification(err.Title, err.Desc, NotificationType.Error);
    }
}