using Tests.TestApp.Services;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Microsoft.Extensions.DependencyInjection;
using Tests.TestApp;
using UI.Services;
using UI.ViewModels.Factories;
using UI.ViewModels.Pages;
using UI.Views;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]
namespace Tests.TestApp;
    
public static class ServicesExtensions {
    public static T Get<T>(this IServiceProvider provider) where T : notnull => provider.GetRequiredService<T>();
}

public class TestAppBuilder {
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions {
            UseHeadlessDrawing = false
        });
}

public class App : Application {
    
    public static readonly IServiceProvider Services = new ServiceCollection()
        .AddTransient<IViewModelFactory, QuestionViewModelFactory>()
        .AddTransient<IErrorHandlerService, TestErrorHandler>()
        .AddTransient<IDialogService, TestDialogService>()
        .AddTransient<INotificationService, TestNotificationManager>()
        .AddSingleton<ISerializerService, TestSerializer>()
        .AddSingleton<IStorageProvider>(services => services.Get<MainWindow>().StorageProvider)
        .AddSingleton<Evaluator>()
        .AddSingleton<MainWindow>()
        .AddSingleton<QuestionsPageViewModel>()
        .AddSingleton<QuestionAddPageViewModel>()
        .AddSingleton<QuestionEditPageViewModel>()
        .AddSingleton<ResultsPageViewModel>()
        .AddSingleton(NavigatorService.FromServiceProvider)
        .BuildServiceProvider();
    
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
    
}