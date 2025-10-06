using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UI.Services;
using UI.ViewModels.Factories;
using UI.ViewModels.Pages;
using UI.Views;

namespace UI;

public static class ServicesExtensions {
    public static T Get<T>(this IServiceProvider provider) where T : notnull => provider.GetRequiredService<T>();
}

public class App : Application {
    
    public static readonly string AppDataDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FileQuestionAssistant");
    
    private readonly IServiceProvider _services = new ServiceCollection()
        .AddSingleton<QuestionTypeMapper>()
        .AddTransient<IViewModelFactory, QuestionViewModelFactory>()
        .AddTransient<IErrorHandlerService, ErrorHandler>()
        .AddSingleton<IDialogService, DialogService>()
        .AddTransient<ISerializerService, QuestionSerializer>()
        .AddSingleton<IStorageProvider>(services => services.Get<MainWindow>().StorageProvider)
        .AddTransient<IStorageService, StorageService>()
        .AddSingleton<Evaluator>()
        .AddSingleton<MainWindow>()
        .AddSingleton<QuestionsPageViewModel>()
        .AddSingleton<QuestionAddPageViewModel>()
        .AddSingleton<QuestionEditPageViewModel>()
        .AddSingleton<ResultsPageViewModel>()
        .AddSingleton(NavigatorService.FromServiceProvider)
        .AddSingleton<INotificationService, NotificationManager>(sp => new NotificationManager(sp.Get<MainWindow>()))
        .BuildServiceProvider();
    
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted() {
        Lang.Lang.Culture = CultureInfo.CurrentCulture;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            desktop.Exit += (_, _) => {
                Log.Information("Disposing all evaluator files...");
                _services.Get<Evaluator>().DisposeAllFiles();
                Log.Information("Shutting down the application...");
            };
            desktop.MainWindow = _services.Get<MainWindow>();
            
            _services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}