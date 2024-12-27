using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Core.Evaluation;
using Core.Questions.Word;
using Tests.TestApp;
using Tests.TestApp.Services;
using Tests.Utils;
using UI.Services;
using UI.ViewModels.Pages;
using UI.Views;
using UI.Views.Pages;

namespace Tests;

[TestFixture]
public class QuestionsPageTests {

    private static ParagraphApplyStyleQuestion NewFakeParagraphQuestion(string name) {
        var ogFile = File.ReadAllBytes(TestConstants.TestFilesDirectory + "OgFile.docx");
        return new ParagraphApplyStyleQuestion("", name, "Description", ogFile, "styleName");
    }
    
    [SetUp]
    public void ClearQuestions() {
        App.Services.Get<Evaluator>().Clear();
        var errHandler = App.Services.Get<IErrorHandlerService>() as TestErrorHandler;
        errHandler!.Errors.Clear();
    }

    [AvaloniaTest]
    public void QuestionsPageTest_QuestionList() {
        var ev = App.Services.Get<Evaluator>();
        ev.AddQuestion(NewFakeParagraphQuestion("Q1"));
        ev.AddQuestion(NewFakeParagraphQuestion("Q2"));
        
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        Assert.That(App.Services.Get<QuestionsPageViewModel>().QuestionsSearch, Has.Count.EqualTo(ev.Questions.Count));
    }
    
    [AvaloniaTest]
    public void QuestionsPageTest_RemoveQuestion() {
        var ev = App.Services.Get<Evaluator>();
        ev.AddQuestion(NewFakeParagraphQuestion("Q1"));
        ev.AddQuestion(NewFakeParagraphQuestion("Q2"));
        
        var window = App.Services.Get<MainWindow>();
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        window.Show();

        while (ev.Questions.Count > 0) {
            Dispatcher.UIThread.RunJobs();
            var btn = window.GetLogicalDescendants().OfType<ListBoxItem>().First().GetLogicalDescendants().OfType<Button>().First();
            btn.Command?.Execute(btn.DataContext);
        }
        
        Dispatcher.UIThread.RunJobs();
        Assert.That(App.Services.Get<Evaluator>().Questions, Has.Count.EqualTo(0));
    }

    [AvaloniaTest]
    public void QuestionPageTest_ClickQuestion() {
        var ev = App.Services.Get<Evaluator>();
        ev.AddQuestion(NewFakeParagraphQuestion("Q1"));
        
        var window = App.Services.Get<MainWindow>();
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        window.Show();
        
        var item = window.GetLogicalDescendants().OfType<ListBoxItem>().First();
        item.Focus();
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
        
        Assert.That(window.GetLogicalDescendants().OfType<ResultsPageView>(), Is.Not.EqualTo(null));
    }
    
    [AvaloniaTest]
    public void QuestionPageTest_OpenQuestionWrong() {
        var storage = App.Services.Get<IStorageService>() as TestStorageService;
        storage!.IsCorrectFile = false;
        
        var window = App.Services.Get<MainWindow>();
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        window.Show();
        
        var btn = window.GetLogicalDescendants().OfType<Button>().First(b => b.Classes.Contains("big-button"));
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
        
        var errHandler = App.Services.Get<IErrorHandlerService>() as TestErrorHandler;
        Assert.That(errHandler!.Errors, Has.Count.EqualTo(1));
    }
    
    [AvaloniaTest]
    public void QuestionPageTest_OpenQuestionCorrect() {
        var storage = App.Services.Get<IStorageService>() as TestStorageService;
        storage!.IsCorrectFile = true;
        
        var window = App.Services.Get<MainWindow>();
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        window.Show();
        
        var btn = window.GetLogicalDescendants().OfType<Button>().First(b => b.Classes.Contains("big-button"));
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
        
        Assert.That(App.Services.Get<Evaluator>().Questions, Has.Count.EqualTo(1));
    }
    
}