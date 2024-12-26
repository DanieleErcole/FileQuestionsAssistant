using System.Drawing;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Core.Evaluation;
using Core.Questions.Word;
using Tests.TestApp;
using UI.Services;
using UI.ViewModels.Pages;
using UI.Views;
using UI.Views.Pages;

namespace Tests;

[TestFixture]
public class QuestionsPageTests {

    private static CreateStyleQuestion NewFakeCreateStyleQuestion(string name) {
        var ogFile = File.ReadAllBytes(WordTests.WordFileDirectory + "OgFile.docx");
        return new CreateStyleQuestion("", name, "Description", ogFile, "styleName", 
            "baseStyleName", "fontName", 12, Color.Blue, "alignment");
    }

    [SetUp]
    public void ClearQuestions() => App.Services.Get<Evaluator>().Clear();

    [AvaloniaTest]
    public void QuestionsPageTest_QuestionList() {
        var ev = App.Services.Get<Evaluator>();
        ev.AddQuestion(NewFakeCreateStyleQuestion("Q1"));
        ev.AddQuestion(NewFakeCreateStyleQuestion("Q2"));
        
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        Assert.That(App.Services.Get<QuestionsPageViewModel>().QuestionsSearch, Has.Count.EqualTo(ev.Questions.Count));
    }
    
    [AvaloniaTest]
    public void QuestionsPageTest_RemoveQuestion() {
        var ev = App.Services.Get<Evaluator>();
        ev.AddQuestion(NewFakeCreateStyleQuestion("Q1"));
        ev.AddQuestion(NewFakeCreateStyleQuestion("Q2"));
        
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
        ev.AddQuestion(NewFakeCreateStyleQuestion("Q1"));
        
        var window = App.Services.Get<MainWindow>();
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        window.Show();
        
        var item = window.GetLogicalDescendants().OfType<ListBoxItem>().First();
        item.Focus();
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
        
        Assert.That(window.GetLogicalDescendants().OfType<ResultsPageView>(), Is.Not.EqualTo(null));
    }
    
}