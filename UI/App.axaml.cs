using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Core.Evaluation;
using Microsoft.Extensions.DependencyInjection;
using UI.Services;
using UI.ViewModels;
using UI.Views;

namespace UI;

public static class NotificationExtensions {
    public static void ShowNotification(this WindowNotificationManager nm, string title, string? message,
        NotificationType type = NotificationType.Information) {
        nm.Show(new Notification {
            Title = title,
            Message = message,
            Type = type,
        });
    }
}

public static class ServicesExtensions {
    public static T Get<T>(this IServiceProvider provider) where T : notnull {
        return provider.GetRequiredService<T>();
    }
}

public partial class App : Application {
    
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        Lang.Lang.Culture = CultureInfo.CurrentCulture;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            var mw = new MainWindow {
                DataContext = new MainWindowViewModel()
            };
            
            var dialogService = new DialogService(mw);
            var services = new ServiceCollection()
                .AddSingleton<NavigatorService>()
                .AddSingleton<ErrorHandler>()
                .AddSingleton(dialogService)
                .AddSingleton<Evaluator>()
                .AddSingleton(new WindowNotificationManager(mw) {
                    Position = NotificationPosition.BottomRight,
                    MaxItems = 3,
                })
                .AddSingleton<QuestionSerializer>()
                .AddSingleton(mw.StorageProvider)
                .BuildServiceProvider();
            
            desktop.MainWindow = mw;
            services.Get<NavigatorService>().Init(mw.MainFrame);
        }

        base.OnFrameworkInitializationCompleted();
    }
}