using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.NUnit;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Word;
using Core.Utils;
using Tests.TestApp;
using Tests.TestApp.Services;
using Tests.Utils;
using UI.Services;
using UI.ViewModels.Pages;
using UI.ViewModels.Questions;
using UI.Views;

namespace Tests;

[TestFixture]
public class ResultsPageTests {
    
    private static List<ContentPresenter> AddFiles(bool isCorrect) {
        var storage = App.Services.Get<IStorageService>() as TestStorageService;
        storage!.IsCorrect = isCorrect;

        var window = App.Services.Get<MainWindow>();
        var btn = window.GetLogicalDescendants().OfType<Button>().First(b => b.Name == "AddFilesBtn");
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
        
        return window
            .GetLogicalDescendants().OfType<ItemsControl>()
            .First(list => list.Name == "FileList")
            .GetLogicalDescendants().OfType<ContentPresenter>()
            .ToList();
    }
    
    [SetUp]
    public void Setup() {
        var storage = App.Services.Get<IStorageService>() as TestStorageService;
        storage!.DocumentTypeToSelect = TestStorageService.DocumentType.Word;
        
        var ev = App.Services.Get<Evaluator>();
        ev.Clear();
        
        var ogFile = new MemoryFile("OgFile.docx", File.ReadAllBytes(TestConstants.TestFilesDirectory + "OgFile.docx"));
        var q = new ParagraphApplyStyleQuestion("", "Q1", "Description", ogFile, "CustomStyle");
        ev.AddQuestion(q);
        
        var errHandler = App.Services.Get<IErrorHandlerService>() as TestErrorHandler;
        errHandler!.Errors.Clear();
        
        App.Services.Get<ResultsPageViewModel>().ToQuestionPage();
        App.Services.Get<NavigatorService>().NavigateTo<ResultsPageViewModel>(q);
        App.Services.Get<MainWindow>().Show();
    }
    
    [AvaloniaTest]
    public void ResultsPageTest_NavigationSuccessful() => 
        Assert.That(App.Services.Get<ResultsPageViewModel>().QuestionVM, Is.Not.EqualTo(null));
    
    [AvaloniaTest]
    public void ResultsPageTest_AddFileCorrect() => Assert.That(AddFiles(true), Has.Count.GreaterThan(0));
    
    [AvaloniaTest]
    public void ResultsPageTest_AddFileWrong() => Assert.Multiple(() => {
        var errHandler = App.Services.Get<IErrorHandlerService>() as TestErrorHandler;
        Assert.That(AddFiles(false), Has.Count.EqualTo(0));
        Assert.That(errHandler!.Errors, Has.Count.EqualTo(1));
    });

    [AvaloniaTest]
    public void ResultsPageTest_RemoveFile() {
        AddFiles(true);
        var window = App.Services.Get<MainWindow>();
        var checkBox = window
            .GetLogicalDescendants().OfType<ItemsControl>()
            .First(list => list.Name == "FileList")
            .GetLogicalDescendants().OfType<CheckBox>()
            .First();

        checkBox.IsChecked = true;
        var btn = window.GetLogicalDescendants().OfType<Button>().First(b => b.Name == "DeleteBtn");
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
        
        Assert.That(App.Services.Get<Evaluator>().Files.First(), Has.Count.EqualTo(0));
    }
    
    [AvaloniaTest]
    public void ResultsPageTest_EvaluateCorrect() {
        AddFiles(true);
        
        var btn = App.Services.Get<MainWindow>().GetLogicalDescendants().OfType<Button>().First(b => b.Name == "EvaluateBtn");
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();

        var fileRes = App.Services.Get<ResultsPageViewModel>().FilesResult!.OfType<FileResultViewModel>().First();
        Assert.That(fileRes.Result!.IsSuccessful, Is.True);
    }
    
    [AvaloniaTest]
    public void ResultsPageTest_EvaluateWrong() {
        var ev = App.Services.Get<Evaluator>();
        var page = App.Services.Get<ResultsPageViewModel>();

        using var stream = File.Open(TestConstants.TestFilesDirectory + "OgFile.docx", FileMode.Open);
        using var wrongFile = new WordFile(stream.Name, stream);
        
        App.Services.Get<Evaluator>().AddFiles(ev.Questions.First(), wrongFile);
        page.FilesResult!.Refresh();
        
        var btn = App.Services.Get<MainWindow>().GetLogicalDescendants().OfType<Button>().First(b => b.Name == "EvaluateBtn");
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();

        var fileRes = App.Services.Get<ResultsPageViewModel>().FilesResult!.OfType<FileResultViewModel>().First();
        Assert.That(fileRes.Result!.IsSuccessful, Is.False);
    }
    
}