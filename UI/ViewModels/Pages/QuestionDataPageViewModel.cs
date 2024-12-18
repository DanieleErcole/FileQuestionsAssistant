using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Questions;
using UI.Services;
using UI.ViewModels.QuestionForms;

namespace UI.ViewModels.Pages;

public abstract partial class QuestionDataPageViewModel : PageViewModelBase {
    
    private int _selectedIndex;
    public int SelectedIndex {
        get => _selectedIndex;
        set {
            _selectedIndex = value;
            Content = IndexToFormViewModel(SelectedIndex);
        }
    }

    [ObservableProperty]
    private QuestionFormBaseVM? _content;

    public string PageTitle { get; }
    public string? SaveButtonText { get; }

    protected QuestionDataPageViewModel(string title, string btnText, IServiceProvider services) : base(services) {
        PageTitle = title;
        SaveButtonText = btnText;
    }
    
    protected QuestionFormBaseVM? IndexToFormViewModel(int questionIndex, AbstractQuestion? question = null) {
        return questionIndex switch {
            0 => question is not null ? new CreateStyleQuestionFormViewModel(_services, question) : new CreateStyleQuestionFormViewModel(_services),
            //1 => question is not null ? new CreateStyleQuestionFormViewModel(_services, question) : new CreateStyleQuestionFormViewModel(_services),
            //2 => question is not null ? new CreateStyleQuestionFormViewModel(_services, question) : new CreateStyleQuestionFormViewModel(_services)
            _ => null
        };
    }

    public void ToQuestionPage() => _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);

    public abstract Task ProcessQuestion();
    
}