using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils.Errors;
using FluentAvalonia.UI.Data;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using UI.Services;
using UI.ViewModels.Questions;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace UI.ViewModels;

public class QuestionsPageViewModel : PageViewModelBase {

    private readonly IServiceProvider _services;

    private string? _searchText;
    public string? SearchText {
        get => _searchText;
        set {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            QuestionsSearch.Refresh();
        }
    }
    
    public IterableCollectionView QuestionsSearch { get; }
    
    private SingleQuestionViewModel FromTypeToViewModel(IQuestion question) {
        return question switch {
            CreateStyleQuestion csq => new CreateStyleQuestionVM(csq, _services),
            _ => throw new UnreachableException()
        };
    }

    public QuestionsPageViewModel(IServiceProvider services) {
        _services = services;
        
        var ev = _services.GetRequiredService<Evaluator>();
        var loadedQuestions = _services.GetRequiredService<QuestionSerializer>().LoadTrackedQuestions() ?? [];
        
        foreach (var q in loadedQuestions)
            ev.AddQuestion(q);
        
        QuestionsSearch = new IterableCollectionView(ev.Questions.Select(FromTypeToViewModel), o => {
            var q = (o as SingleQuestionViewModel)!;
            return string.IsNullOrWhiteSpace(SearchText) ||
                   q.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                   q.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        });
    }
    
    public override void OnNavigatedTo() {
        QuestionsSearch.Refresh();
    }
    
    #region Button commands

        public async Task OpenQuestion() {
            var files = await _services.Get<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions {
                AllowMultiple = false,
                FileTypeFilter = [_services.Get<QuestionSerializer>().FileType]
            });
            if (!files.Any()) return;
            
            try {
                var serializer = _services.Get<QuestionSerializer>();
                // Add the opened file to the tracked ones
                await serializer.AddTrackedQuestion(files[0]);
                // Load the question file
                if (await serializer.Load(files[0]) is { } q)
                    _services.Get<Evaluator>().AddQuestion(q);
            } catch (Exception e) {
                _services.Get<ErrorHandler>().ShowError(e);
                return;
            }
            
            QuestionsSearch.Refresh();
            _services.Get<WindowNotificationManager>()
                .Show(new Notification("Opened a question", $"Opened question file: {files[0].Name} and added to the tracked files"));
            //_services.GetRequiredService<NavigatorService>().NavigateTo(NavigatorService.Results);
        }

        public void AddQuestionBtn() {
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.QuestionForm);
        }

        public async Task DeleteQuestion(object param) {
            var question = (param as SingleQuestionViewModel)!;
            try {
                await _services.Get<QuestionSerializer>().RemoveTrackedQuestion(question);
                _services.Get<Evaluator>().RemoveQuestion(question.Index);
            } catch (FileError e) {
                //TODO: handle the case where the tracked questions file is deleted while running the application
                //TODO: add exceptions matching the current implemented error cases
                _services.Get<ErrorHandler>().ShowError(e);
                //_services.Get<WindowNotificationManager>().ShowError("Unable to untrack question", $"Error while opening {e.Filename}");
            }
            QuestionsSearch.Refresh();
        }

        public void OnSelectedQuestion(SelectionChangedEventArgs e) {
            if (e.AddedItems.Count != 1) return;
            var selected = e.AddedItems[0] as SingleQuestionViewModel;
            _services.Get<WindowNotificationManager>()
                .Show(new Notification("Selected a question", $"Selected question: {selected!.Name}"));
        }
    
    #endregion
}