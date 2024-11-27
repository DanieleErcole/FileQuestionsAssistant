using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Core.Evaluation;
using Core.FileHandling;
using Microsoft.Extensions.DependencyInjection;
using UI.Services;
using UI.ViewModels;
using UI.Views;

namespace UI;

public static class NotificationExtensions {
    public static void ShowError(this WindowNotificationManager nm, string title, string message) {
        nm.Show(new Notification {
            Title = title,
            Message = message,
            Type = NotificationType.Error,
        });
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
            var serializer = new QuestionSerializer();
            var services = new ServiceCollection()
                .AddSingleton<NavigatorService>()
                .AddSingleton(dialogService)
                .AddSingleton<Evaluator<WordFile>>()
                .AddSingleton(new WindowNotificationManager(mw) {
                    Position = NotificationPosition.BottomRight,
                    MaxItems = 3,
                })
                .AddSingleton(serializer)
                .AddSingleton(mw.StorageProvider)
                //TODO: Add other file evaluators
                .BuildServiceProvider();
            
            serializer.Init(services);
            desktop.MainWindow = mw;
            services.GetRequiredService<NavigatorService>().Init(mw.MainFrame, services);
        }

        base.OnFrameworkInitializationCompleted();
    }
}