using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Utils.Errors;
using FluentAvalonia.UI.Data;
using UI.Services;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public class QuestionsPageViewModel : PageViewModel {

    private readonly DialogService _dialogService;
    private readonly IStorageProvider _storageProvider;
    
    private string? _searchText;
    public string? SearchText {
        get => _searchText;
        set {
            SetProperty(ref _searchText, value);
            QuestionsSearch.Refresh();
        }
    }
    
    public IterableCollectionView QuestionsSearch { get; }

    public QuestionsPageViewModel(NavigatorService navService, ErrorHandler errorHandler, QuestionSerializer serializer, 
        Evaluator evaluator, DialogService dialogService, IStorageProvider storageProvider) 
        : base(navService, errorHandler, serializer, evaluator) {
        _dialogService = dialogService;
        _storageProvider = storageProvider;
        var loadedQuestions = Serializer.LoadTrackedQuestions() ?? [];
        
        foreach (var q in loadedQuestions)
            Evaluator.AddQuestion(q);
        
        QuestionsSearch = new IterableCollectionView(Evaluator.Questions.Select(q => q.ToViewModel(Evaluator, ErrorHandler, _storageProvider)), o => {
            var q = (o as SingleQuestionViewModel)!;
            return string.IsNullOrWhiteSpace(SearchText) ||
                   q.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                   q.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        });
    }

    public override void OnNavigatedTo(object? param = null) => QuestionsSearch.Refresh();
    
    public async Task OpenQuestion() {
        var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
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
                NavigatorService.NavigateTo(NavigatorService.Results, q);
            }
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
    }

    public void AddQuestionBtn() => NavigatorService.NavigateTo(NavigatorService.QuestionAddForm);

    public void EditQuestionBtn(object param) {
        var vm = (param as SingleQuestionViewModel)!;
        NavigatorService.NavigateTo(NavigatorService.QuestionEditForm, vm.Question);
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

    public void OnSelectedQuestion(SelectionChangedEventArgs e) {
        if (e.AddedItems.Count != 1) return;
        var selected = e.AddedItems[0] as SingleQuestionViewModel;
        NavigatorService.NavigateTo(NavigatorService.Results, selected!.Question);
    }
    
}