using System;
using Avalonia.Controls.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace UI.Services;

public class ErrorHandler {

    private IServiceProvider _services;

    public void Init(IServiceProvider services) {
        _services = services;
    }

    public void ShowError(Exception ex) {
        Console.WriteLine(ex);
        var err = ex as UIException ?? UIException.FromException(ex);
        _services.GetRequiredService<WindowNotificationManager>().ShowError(err.Title, err.Desc);
    }
    
}