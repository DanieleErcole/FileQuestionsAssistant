using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Utils.Errors;
using FluentAvalonia.UI.Data;
using UI.Services;
using UI.Utils;
using UI.ViewModels.Factories;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public class QuestionsPageViewModel : PageViewModel {

    private readonly IDialogService _dialogService;
    
    private string? _searchText;
    public string? SearchText {
        get => _searchText;
        set {
            SetProperty(ref _searchText, value);
            QuestionsSearch.Refresh();
        }
    }
    
    public IterableCollectionView QuestionsSearch { get; }

    public QuestionsPageViewModel(NavigatorService navService, IErrorHandlerService errorHandler, ISerializerService serializer, Evaluator evaluator, IDialogService dialogService,
        IStorageService storageService, IViewModelFactory vmFactory) : base(navService, errorHandler, serializer, evaluator, storageService, vmFactory) {
        _dialogService = dialogService;
        
        var loadedQuestions = Serializer.LoadTrackedQuestions() ?? [];
        foreach (var q in loadedQuestions.Where(q => q is not null))
            Evaluator.AddQuestion(q!);
        
        QuestionsSearch = new IterableCollectionView(Evaluator.Questions.Select(vmFactory.NewQuestionVm), o => {
            var q = (o as SingleQuestionViewModel)!;
            return string.IsNullOrWhiteSpace(SearchText) ||
                   q.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                   q.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        });
    }

    public override void OnNavigatedTo(object? param = null) => QuestionsSearch.Refresh();
    
    public async Task OpenQuestion() {
        var files = await StorageService.GetFilesAsync(new FilePickerOpenOptions {
            AllowMultiple = false,
            FileTypeFilter = [QuestionSerializer.FileType]
        });
        if (!files.Any()) return;
        
        var filePath = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
        try {
            if (await Serializer.Load(filePath) is { } q) {
                if (Evaluator.Questions.Any(e => e.Path == q.Path))
                    throw new UnableToOpenQuestion();
                
                Evaluator.AddQuestion(q);
                await Serializer.UpdateTrackingFile();
                
                NavigatorService.NavigateTo<ResultsPageViewModel>(q);
            }
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
        QuestionsSearch.Refresh();
    }

    public void AddQuestionBtn() => NavigatorService.NavigateTo<QuestionAddPageViewModel>();

    public void EditQuestionBtn(object param) {
        var vm = (param as SingleQuestionViewModel)!;
        NavigatorService.NavigateTo<QuestionEditPageViewModel>(vm.Question);
    }

    public async Task DeleteQuestion(object param) {
        var question = (param as SingleQuestionViewModel)!;
        if (!await _dialogService.ShowYesNoDialog(Lang.Lang.DeleteDialogTitle, Lang.Lang.DeleteDialogMessage + $"\n{question.Path}")) 
            return;
            
        try {
            Evaluator.RemoveQuestion(question.Question);
            await Serializer.UpdateTrackingFile();
        } catch (FileError e) {
            ErrorHandler.ShowError(e);
        }
        QuestionsSearch.Refresh();
    }

    public void OnQuestionSelected(object? sender, PointerPressedEventArgs _) {
        var item = sender as StyledElement;
        var selected = (item!.DataContext as SingleQuestionViewModel)!;
        NavigatorService.NavigateTo<ResultsPageViewModel>(selected.Question);
    }
    
}