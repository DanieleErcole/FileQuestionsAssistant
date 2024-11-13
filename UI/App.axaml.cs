using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Core.Evaluation;
using Core.FileHandling;
using Microsoft.Extensions.DependencyInjection;
using UI.Services;
using UI.ViewModels;
using UI.Views;

namespace UI;

public partial class App : Application {
    
    private readonly IServiceProvider _services = new ServiceCollection()
        .AddSingleton<NavigatorService>()
        .AddSingleton<Evaluator<WordFile>>()
        // Add other files evaluator
        .BuildServiceProvider();
    
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        Lang.Localization.Culture = CultureInfo.CurrentCulture;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow {
                DataContext = new MainWindowViewModel(_services),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}