using System;
using Avalonia.Controls.Notifications;
using Microsoft.Extensions.DependencyInjection;
using UI.Utils;

namespace UI.Services;

public class ErrorHandler(INotificationService notificationManager) : IErrorHandlerService {
    public void ShowError(Exception ex) {
        Console.WriteLine(ex);
        var err = ex as UIException ?? UIException.FromException(ex);
        notificationManager.ShowNotification(err.Title, err.Desc, NotificationType.Error);
    }
}