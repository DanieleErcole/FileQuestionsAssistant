using System;
using Avalonia.Controls.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels.Questions;

public class ErrorHandler {

    private IServiceProvider _services;

    public void Init(IServiceProvider services) {
        _services = services;
    }

    public void ShowError(Exception ex) {
        var err = ex as UIException ?? UIException.FromException(ex);
        _services.GetRequiredService<WindowNotificationManager>().ShowError(err.Title, err.Desc);
    }
    
}