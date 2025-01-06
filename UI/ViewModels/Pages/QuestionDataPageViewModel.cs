using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.Questions;
using UI.Services;
using UI.ViewModels.Factories;
using UI.ViewModels.QuestionForms;

namespace UI.ViewModels.Pages;

public abstract partial class QuestionDataPageViewModel(string title, string btnText, NavigatorService navService, IErrorHandlerService errorHandler, ISerializerService serializer, 
    Evaluator evaluator, IStorageService storageService, IViewModelFactory vmFactory) : PageViewModel(navService, errorHandler, serializer, evaluator, storageService, vmFactory) {

    // Note: edit when adding new question types
    public string[] QuestionTypes { get; } = [
        Lang.Lang.CreateStyleQuestionName,
        Lang.Lang.ParagraphApplyStyleQuestionName,
        Lang.Lang.PptxShapeInsertQuestionName
    ];

    private int _selectedIndex;
    public int SelectedIndex {
        get => _selectedIndex;
        set {
            SetProperty(ref _selectedIndex, value);
            Content = ViewModelFactory.NewQuestionFormVm((QuestionTypeIndex) SelectedIndex);
        }
    }

    [ObservableProperty]
    private QuestionFormBaseVM? _content;

    public string PageTitle { get; } = title;
    public string? SaveButtonText { get; } = btnText;

    public void ToQuestionPage() => NavigatorService.NavigateTo<QuestionsPageViewModel>();

    public abstract Task ProcessQuestion();
    
}