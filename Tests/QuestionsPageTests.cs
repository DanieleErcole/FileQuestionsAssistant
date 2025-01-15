using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Core.Evaluation;
using Core.Questions.Word;
using Core.Utils;
using Tests.TestApp;
using Tests.TestApp.Services;
using Tests.Utils;
using UI.Services;
using UI.ViewModels.Pages;
using UI.Views;
using UI.Views.Controls;
using UI.Views.Pages;

namespace Tests;

[TestFixture]
public class QuestionsPageTests {

    private static ParagraphApplyStyleQuestion NewFakeParagraphQuestion(string name) {
        var ogFile = new MemoryFile("OgFile.docx", File.ReadAllBytes(TestConstants.TestFilesDirectory + "OgFile.docx"));
        return new ParagraphApplyStyleQuestion(name, name, "Description", ogFile, "styleName");
    }

    private static void DeleteQuestion(string name) {
        var btn = App.Services.Get<MainWindow>()
            .GetLogicalDescendants().OfType<QuestionList>()
            .First()
            .GetLogicalDescendants().OfType<Grid>()
            .First(item => item.GetLogicalDescendants().OfType<TextBlock>().FirstOrDefault()?.Text == name)
            .GetLogicalDescendants()
            .OfType<Button>()
            .First();
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
    }
    
    private static void RefreshList() {
        App.Services.Get<QuestionsPageViewModel>().QuestionsSearch.Refresh();
        Dispatcher.UIThread.RunJobs();
    }
    
    private static void OpenQuestion(bool isCorrect) {
        var storage = App.Services.Get<IStorageService>() as TestStorageService;
        storage!.IsCorrect = isCorrect;
        
        var btn = App.Services.Get<MainWindow>().GetLogicalDescendants().OfType<Button>().First(b => b.Classes.Contains("big-button"));
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
        
        RefreshList();
    }
    
    [SetUp]
    public void Setup() {
        var storage = App.Services.Get<IStorageService>() as TestStorageService;
        storage!.DocumentTypeToSelect = TestStorageService.DocumentType.Question;
        
        App.Services.Get<Evaluator>().Clear();
        App.Services.Get<ISerializerService>().UpdateTrackingFile();
        
        var errHandler = App.Services.Get<IErrorHandlerService>() as TestErrorHandler;
        errHandler!.Errors.Clear();
        
        App.Services.Get<MainWindow>().Show();
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
    }

    [AvaloniaTest]
    public void QuestionsPageTest_QuestionList() {
        var ev = App.Services.Get<Evaluator>();
        ev.AddQuestion(NewFakeParagraphQuestion("Q1"));
        ev.AddQuestion(NewFakeParagraphQuestion("Q2"));
        
        RefreshList();
        Dispatcher.UIThread.RunJobs();
        Assert.That(App.Services.Get<QuestionsPageViewModel>().QuestionsSearch, Has.Count.EqualTo(ev.Questions.Count));
    }
    
    [AvaloniaTest]
    public void QuestionsPageTest_RemoveQuestion() {
        var ev = App.Services.Get<Evaluator>();
        ev.AddQuestion(NewFakeParagraphQuestion("Q1"));
        ev.AddQuestion(NewFakeParagraphQuestion("Q2"));
        RefreshList();
        
        DeleteQuestion("Q1");
        DeleteQuestion("Q2");
        Assert.That(App.Services.Get<Evaluator>().Questions, Has.Count.EqualTo(0));
    }

    [AvaloniaTest]
    public void QuestionPageTest_ClickQuestion() {
        var ev = App.Services.Get<Evaluator>();
        ev.AddQuestion(NewFakeParagraphQuestion("Q1"));
        RefreshList();
        
        var window = App.Services.Get<MainWindow>();
        var item = window
            .GetLogicalDescendants().OfType<QuestionList>()
            .First()
            .GetLogicalDescendants().OfType<Grid>()
            .First();
        item.Focus();
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
        
        Assert.That(window.GetLogicalDescendants().OfType<ResultsPageView>(), Is.Not.EqualTo(null));
    }
    
    [AvaloniaTest]
    public void QuestionPageTest_OpenQuestionWrong() {
        OpenQuestion(false);
        var errHandler = App.Services.Get<IErrorHandlerService>() as TestErrorHandler;
        Assert.That(errHandler!.Errors, Has.Count.EqualTo(1));
    }
    
    [AvaloniaTest]
    public void QuestionPageTest_OpenQuestionCorrect() {
        OpenQuestion(true);
        Assert.That(App.Services.Get<Evaluator>().Questions, Has.Count.EqualTo(1));
    }
    
}