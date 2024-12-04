using System;
using System.Threading.Tasks;
using Core.Evaluation;
using ReactiveUI;
using UI.Services;
using UI.ViewModels.QuestionForms;

namespace UI.ViewModels.Pages;

public class QuestionDataPageViewModel(IServiceProvider services) : PageViewModelBase(services) {

    private int _selectedIndex;
    public int SelectedIndex {
        get => _selectedIndex;
        set {
            _selectedIndex = value;
            Content = _selectedIndex switch {
                0 => new CreateStyleQuestionFormViewModel(_services),
                _ => null
            };
        }
    }
    
    private QuestionFormBaseVM? _content;
    public QuestionFormBaseVM? Content {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public override void OnNavigatedTo() {
        SelectedIndex = 0;
    }

    public void ToQuestionPage() {
        _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
    }

    public async Task NewQuestion() {
        if (Content is null) 
            return;
        try {
            var q = await Content.CreateQuestion();
            if (q is null) return;
            _services.Get<Evaluator>().AddQuestion(q);
        } catch (Exception e) {
            _services.Get<ErrorHandler>().ShowError(e);
            return;
        }
        // Test: I should navigate to the results page
        _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
    }
    
}