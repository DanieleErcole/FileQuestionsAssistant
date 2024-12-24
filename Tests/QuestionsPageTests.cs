using System.Drawing;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
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
        
        foreach (var item in window.GetLogicalDescendants().OfType<ListBoxItem>()) {
            var btn = item.GetLogicalDescendants().OfType<Button>().First();
            btn.Command?.Execute(btn.DataContext);
            Dispatcher.UIThread.RunJobs();
        }
        
        Assert.That(App.Services.Get<Evaluator>().Questions, Has.Count.EqualTo(0));
    }
    
}