using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Utils.Errors;
using FluentAvalonia.UI.Data;
using ReactiveUI;
using UI.Services;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public class QuestionsPageViewModel : PageViewModelBase {

    private string? _searchText;
    public string? SearchText {
        get => _searchText;
        set {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            QuestionsSearch.Refresh();
        }
    }
    
    public IterableCollectionView QuestionsSearch { get; }

    public QuestionsPageViewModel(IServiceProvider services) : base(services) {
        var ev = _services.Get<Evaluator>();
        var loadedQuestions = _services.Get<QuestionSerializer>().LoadTrackedQuestions() ?? [];
        
        foreach (var q in loadedQuestions)
            ev.AddQuestion(q);
        
        QuestionsSearch = new IterableCollectionView(ev.Questions.Select(q => q.ToViewModel(_services)), o => {
            var q = (o as SingleQuestionViewModel)!;
            return string.IsNullOrWhiteSpace(SearchText) ||
                   q.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                   q.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        });
    }

    public override void OnNavigatedTo(object? param = null) {
        QuestionsSearch.Refresh();
    }
    
    #region Button commands

        public async Task OpenQuestion() {
            var files = await _services.Get<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions {
                AllowMultiple = false,
                FileTypeFilter = [QuestionSerializer.FileType]
            });
            if (!files.Any()) return;
            
            var filePath = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
            try {
                var serializer = _services.Get<QuestionSerializer>();
                if (await serializer.Load(filePath) is { } q) {
                    await serializer.AddTrackedQuestion(filePath);
                    _services.Get<Evaluator>().AddQuestion(q);
                    _services.Get<NavigatorService>().NavigateTo(NavigatorService.Results, q);
                }
            } catch (Exception e) {
                _services.Get<ErrorHandler>().ShowError(e);
            }
        }

        public void AddQuestionBtn() {
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.QuestionForm);
        }

        public async Task DeleteQuestion(object param) {
            var question = (param as SingleQuestionViewModel)!;
            if (!await _services.Get<DialogService>().ShowYesNoDialog(Lang.Lang.DeleteDialogTitle, Lang.Lang.DeleteDialogMessage + $"\n{question.Path}")) 
                return;
            
            try {
                await _services.Get<QuestionSerializer>().RemoveTrackedQuestion(question);
                _services.Get<Evaluator>().RemoveQuestion(question.Index);
            } catch (FileError e) {
                //TODO: handle the case where the tracked questions file is deleted while running the application
                _services.Get<ErrorHandler>().ShowError(e);
            }
            QuestionsSearch.Refresh();
        }

        public void OnSelectedQuestion(SelectionChangedEventArgs e) {
            if (e.AddedItems.Count != 1) return;
            var selected = e.AddedItems[0] as SingleQuestionViewModel;
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Results, _services.Get<Evaluator>().Questions[selected!.Index]);
        }
    
    #endregion
}