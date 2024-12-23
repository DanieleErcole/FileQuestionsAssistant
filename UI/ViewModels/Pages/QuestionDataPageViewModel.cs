using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.Questions;
using UI.Services;
using UI.ViewModels.Factories;
using UI.ViewModels.QuestionForms;

namespace UI.ViewModels.Pages;

public abstract partial class QuestionDataPageViewModel(
    string title,
    string btnText,
    NavigatorService navService,
    IErrorHandlerService errorHandler,
    ISerializerService serializer,
    Evaluator evaluator,
    IStorageProvider storageProvider,
    IViewModelFactory vmFactory)
    : PageViewModel(navService, errorHandler, serializer, evaluator, storageProvider, vmFactory) {
    
    private int _selectedIndex;
    public int SelectedIndex {
        get => _selectedIndex;
        set {
            _selectedIndex = value;
            Content = ViewModelFactory.NewQuestionFormVm(SelectedIndex);
        }
    }

    [ObservableProperty]
    private QuestionFormBaseVM? _content;

    public string PageTitle { get; } = title;
    public string? SaveButtonText { get; } = btnText;

    public void ToQuestionPage() => NavigatorService.NavigateTo<QuestionsPageViewModel>();

    public abstract Task ProcessQuestion();
    
}