using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Core.FileHandling;
using Core.Questions.Word;
using FluentAvalonia.UI.Data;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using UI.Services;
using UI.ViewModels.Questions;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace UI.ViewModels;

public class QuestionsPageViewModel : ViewModelBase {

    private readonly IServiceProvider _services;

    private string? _searchText;
    public string? SearchText {
        get => _searchText;
        set {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            QuestionsSearch.Refresh();
        }
    }

    private readonly ObservableCollection<SingleQuestionViewModel> _questions = [];
    public IterableCollectionView QuestionsSearch { get; }

    public QuestionsPageViewModel(IServiceProvider services) {
        _services = services;
        
        QuestionsSearch = new IterableCollectionView(_questions, o => {
            var q = (o as SingleQuestionViewModel)!;
            return string.IsNullOrWhiteSpace(SearchText) ||
                   q.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                   q.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        });

        var loadedQuestions = _services.GetRequiredService<QuestionSerializer>().LoadTrackedQuestions() ?? [];
        foreach (var q in loadedQuestions) {
            _questions.Add(q switch {
                CreateStyleQuestion csq => new CreateStyleQuestionVM(csq, _services),
                _ => throw new UnreachableException()
            });
        }
    }

    public void AddQuestionBtn() {
        _services.GetRequiredService<NavigatorService>().NavigateTo(NavigatorService.QuestionForm);
    }
    
    public async Task AddQuestionTest() {
        var q = new CreateStyleQuestion("Nome1", "Desc1", "", "CustomStyle", "Normal", 
            "Consolas", 12, Color.Fuchsia, "center");
        var qvm = new CreateStyleQuestionVM(q, _services);
        _questions.Add(qvm);
        var file = await _services.GetRequiredService<IStorageProvider>()
            .SaveFilePickerAsync(new FilePickerSaveOptions {
                Title = "Save Text File",
                FileTypeChoices = [
                    new FilePickerFileType("JSON file") {
                        Patterns = ["*.json"],
                        AppleUniformTypeIdentifiers = ["public.json"],
                        MimeTypes = ["application/json"]
                    }
                ],
            });
        if (file is not null)
            await _services.GetRequiredService<QuestionSerializer>().Create<CreateStyleQuestion, WordFile>(file, q);
    }

    public void DeleteQuestion(object param) {
        var question = param as SingleQuestionViewModel;
        var deletedIndex = question!.Index;
        
        question.OnRemove();
        _questions.Remove(question);
        
        foreach (var q in _questions)
            if (q.Index > deletedIndex)
                q.Index -= 1;
    }

    public void OnSelectedQuestion(SelectionChangedEventArgs e) {
        if (e.AddedItems.Count != 1) return;
        var selected = e.AddedItems[0] as SingleQuestionViewModel;
        _services.GetRequiredService<WindowNotificationManager>()
            .Show(new Notification("Selected a question", $"Selected question: {selected!.Name}"));
    }
    
}