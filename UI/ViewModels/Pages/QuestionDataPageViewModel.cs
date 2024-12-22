using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.Questions;
using UI.Services;
using UI.ViewModels.QuestionForms;

namespace UI.ViewModels.Pages;

public abstract partial class QuestionDataPageViewModel : PageViewModel {

    private readonly IStorageProvider _storageProvider;
    
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

    protected QuestionDataPageViewModel(string title, string btnText, 
        NavigatorService navService, ErrorHandler errorHandler, QuestionSerializer serializer, Evaluator evaluator, IStorageProvider storageProvider) 
        : base(navService, errorHandler, serializer, evaluator) {
        _storageProvider = storageProvider;
        PageTitle = title;
        SaveButtonText = btnText;
    }
    
    protected QuestionFormBaseVM? IndexToFormViewModel(int questionIndex, AbstractQuestion? question = null) {
        return questionIndex switch {
            0 => question is not null ? new CreateStyleQuestionFormViewModel(ErrorHandler, _storageProvider, question) 
                : new CreateStyleQuestionFormViewModel(ErrorHandler, _storageProvider),
            1 => question is not null ? new ParagraphApplyStyleQuestionFormViewModel(ErrorHandler, _storageProvider, question) 
                : new ParagraphApplyStyleQuestionFormViewModel(ErrorHandler, _storageProvider),
            //2 => question is not null ? new CreateStyleQuestionFormViewModel(_services, question) : new CreateStyleQuestionFormViewModel(_services)
            _ => null
        };
    }

    public void ToQuestionPage() => NavigatorService.NavigateTo(NavigatorService.Questions);

    public abstract Task ProcessQuestion();
    
}