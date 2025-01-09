using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils;
using Tests.TestApp;
using Tests.TestApp.Services;
using Tests.Utils;
using UI.Services;
using UI.ViewModels.Pages;
using UI.ViewModels.QuestionForms.Word;
using UI.Views;
using UI.Views.Controls;

namespace Tests;

[TestFixture]
public class QuestionEditPageTests {
    
    [SetUp]
    public void Setup() {
        var ev = App.Services.Get<Evaluator>();
        var errHandler = App.Services.Get<IErrorHandlerService>() as TestErrorHandler;
        errHandler!.Errors.Clear();
        
        App.Services.Get<Evaluator>().Clear();
        App.Services.Get<ISerializerService>().UpdateTrackingFile();

        var window = App.Services.Get<MainWindow>();
        window.Show();
        
        var ogFile = new MemoryFile("OgFile.docx", File.ReadAllBytes(TestConstants.TestFilesDirectory + "OgFile.docx"));
        ev.AddQuestion(new ParagraphApplyStyleQuestion("Q1", "Q1", "Description", ogFile, "styleName"));
        App.Services.Get<NavigatorService>().NavigateTo<QuestionsPageViewModel>();
        Dispatcher.UIThread.RunJobs();
        
        var btn = window
            .GetLogicalDescendants().OfType<QuestionList>()
            .First()
            .GetLogicalDescendants().OfType<Grid>()
            .First()
            .GetLogicalDescendants().OfType<Button>()
            .Last();
        btn.Command?.Execute(btn.DataContext);
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaTest]
    public void QuestionEditPage_NavigationSuccessful() {
        var form = App.Services.Get<QuestionEditPageViewModel>().Content as ParagraphApplyStyleQuestionFormViewModel;
        Assert.That(form!.Name, Is.EqualTo("Q1"));
    }
    
    [AvaloniaTest]
    public async Task QuestionEditPage_SaveEditsCorrect() {
        var page = App.Services.Get<QuestionEditPageViewModel>();
        var form = page.Content as ParagraphApplyStyleQuestionFormViewModel;
        form!.Name = "QEdited";
        
        await page.ProcessQuestion();
        Dispatcher.UIThread.RunJobs();
        
        var q = App.Services.Get<Evaluator>().Questions.OfType<AbstractQuestion>().First();
        Assert.That(q.Name, Is.EqualTo("QEdited"));
    }
    
    [AvaloniaTest]
    public void QuestionEditPage_SaveEditsWrong() {
        var form = App.Services.Get<QuestionEditPageViewModel>().Content as ParagraphApplyStyleQuestionFormViewModel;
        form!.Name = "";
        
        var btn = App.Services.Get<MainWindow>().GetLogicalDescendants().OfType<Button>().First(b => b.Classes.Contains("accent"));
        btn.Command?.Execute(btn.DataContext);
        
        Assert.That(form.IsError, Is.True);
    }
    
}