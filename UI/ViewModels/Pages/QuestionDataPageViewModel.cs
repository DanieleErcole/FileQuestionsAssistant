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

    public override void OnNavigatedTo(object? param = null) => SelectedIndex = 0;

    public void ToQuestionPage() => _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);

    public async Task NewQuestion() {
        if (Content is null) 
            return;
        try {
            var q = Content.CreateQuestion();
            if (q is null) return;

            var ev = _services.Get<Evaluator>();
            var index = ev.Questions.FindIndex(x => x.Path == q.Path);
            var serializer = _services.Get<QuestionSerializer>();
            
            await serializer.Save(q.Path, q);
            if (index != -1) 
                ev.RemoveQuestion(index);
            ev.AddQuestion(q);
            
            await serializer.UpdateTrackingFile();
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Results, q);
        } catch (Exception e) {
            _services.Get<ErrorHandler>().ShowError(e);
        }
    }
    
}