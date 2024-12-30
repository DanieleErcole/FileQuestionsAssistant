using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Core.Evaluation;
using Tests.TestApp;
using Tests.TestApp.Services;
using Tests.Utils;
using UI.Services;
using UI.ViewModels.Pages;
using UI.ViewModels.QuestionForms;
using UI.Views;

namespace Tests;

[TestFixture]
public class QuestionAddPageTests {
    
    [SetUp]
    public void Setup() {
        var storage = App.Services.Get<IStorageService>() as TestStorageService;
        storage!.DocumentTypeToSelect = TestStorageService.DocumentType.Word;
        storage.IsCorrect = null;
        
        var ev = App.Services.Get<Evaluator>();
        ev.Clear();
        
        var errHandler = App.Services.Get<IErrorHandlerService>() as TestErrorHandler;
        errHandler!.Errors.Clear();
        
        App.Services.Get<NavigatorService>().NavigateTo<QuestionAddPageViewModel>();
        App.Services.Get<MainWindow>().Show();
        Dispatcher.UIThread.RunJobs();
    }
    
    [AvaloniaTest]
    public void QuestionAddPage_SubmitWrong() {
        var btn = App.Services.Get<MainWindow>().GetLogicalDescendants().OfType<Button>().First(b => b.Classes.Contains("accent"));
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
        
        Assert.That(App.Services.Get<QuestionAddPageViewModel>().Content!.IsError, Is.True);
    }
    
    [AvaloniaTest]
    public async Task QuestionAddPage_SubmitCorrect() {
        var window = App.Services.Get<MainWindow>();
        window.GetLogicalDescendants().OfType<ComboBox>().First().SelectedIndex = 1; // Paragraph apply style question
        
        var page = App.Services.Get<QuestionAddPageViewModel>().Content as ParagraphApplyStyleQuestionFormViewModel;
        await page!.UploadOgFile();
        page.Name = "Q1";
        page.Desc = "Description";
        page.Path = $"{TestConstants.TestFilesDirectory}FileToSave.json";
        page.StyleNameSelected = "Normal";
        
        var btn = window.GetLogicalDescendants().OfType<Button>().First(b => b.Classes.Contains("accent"));
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
        
        Assert.That(App.Services.Get<Evaluator>().Questions, Has.Count.EqualTo(1));
    }
    
}