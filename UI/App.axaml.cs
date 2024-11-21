using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Microsoft.Extensions.DependencyInjection;
using UI.Services;
using UI.ViewModels;
using UI.Views;

namespace UI;

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

            var mw = new MainWindow();
            var services = new ServiceCollection()
                .AddSingleton<NavigatorService>()
                .AddSingleton<Evaluator<WordFile>>()
                .AddSingleton(mw.StorageProvider)
                //TODO: Add other files evaluator
                .BuildServiceProvider();
            
            mw.DataContext = new MainWindowViewModel(services);
            desktop.MainWindow = mw;
        }

        base.OnFrameworkInitializationCompleted();
    }
}