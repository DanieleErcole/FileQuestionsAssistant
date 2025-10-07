using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using UI.Services;
using UI.Utils;
using UI.ViewModels.Factories;
using UI.ViewModels.QuestionForms;

namespace UI.ViewModels.Pages;

public abstract partial class QuestionFormPageViewModel(
        string title, 
        string btnText, 
        NavigatorService navService, 
        IErrorHandlerService errorHandler, 
        ISerializerService serializer, 
        Evaluator evaluator, 
        IStorageService storageService, 
        QuestionTypeMapper mapper, 
        IViewModelFactory vmFactory
    ) : PageViewModel(navService, errorHandler, serializer, evaluator, storageService, vmFactory) {

    protected readonly QuestionTypeMapper Mapper = mapper;
    
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
            Content = ViewModelFactory.NewQuestionFormVm(Mapper.TypeFromIndex(_selectedIndex));
        }
    }
    
    private string? _errorMsg;
    public string? ErrorMsg {
        get => _errorMsg;
        set {
            SetProperty(ref _errorMsg, value);
            OnPropertyChanged(nameof(IsError));
        }
    }
    public bool IsError => ErrorMsg is not null;

    [ObservableProperty]
    private QuestionFormVMBase? _content;

    public string PageTitle { get; } = title;
    public string? SaveButtonText { get; } = btnText;

    public void ToQuestionPage() {
        NavigatorService.NavigateTo<QuestionsPageViewModel>();
        ErrorMsg = null;
    }
    
    public void CloseErr() {
        ErrorMsg = null;
    }

    public async void Submit() {
        try {
            await ProcessQuestion();
        } catch (FormError f) {
            ErrorMsg = f.Desc;
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
    }
    
    public abstract Task ProcessQuestion();
    
}