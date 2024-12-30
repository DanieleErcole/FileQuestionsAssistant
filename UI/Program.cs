using Avalonia;
using System;
using System.IO;
using Serilog;

namespace UI;

sealed class Program {
    
    private static string LogDirectoryPath => Path.Combine(App.AppDataDirectoryPath, "logs");
    private static string LogFilePath => Path.Combine(LogDirectoryPath, "log.txt");
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        if (!Directory.Exists(App.AppDataDirectoryPath))
            Directory.CreateDirectory(App.AppDataDirectoryPath);
        if (!Directory.Exists(LogDirectoryPath))
            Directory.CreateDirectory(LogDirectoryPath);
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(LogFilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        } catch (Exception e) {
            Log.Fatal(e, "Application terminated unexpectedly");
        } finally {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    
}
