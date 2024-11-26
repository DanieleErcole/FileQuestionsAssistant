using System;
using System.Collections.ObjectModel;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Core.Questions.Word;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using UI.ViewModels.Questions;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace UI.ViewModels;

public class QuestionsPageViewModel : ViewModelBase {

    private IServiceProvider _services;
    
    private ObservableCollection<SingleQuestionViewModel> _questions = [];
    public ObservableCollection<SingleQuestionViewModel> Questions {
        get => _questions; 
        set => this.RaiseAndSetIfChanged(ref _questions, value);
    }

    public QuestionsPageViewModel(IServiceProvider services) {
        _services = services;
        // Solo per vedere il risultato
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("Nome1", "Desc1", "CustomStyle", "", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("Nome2", null,"CustomStyle", "", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("Nome3", "Desc3","CustomStyle", "", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("Nome4", null,"CustomStyle", "", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("Nome5", "Desc1", "CustomStyle", "", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("Nome6", null,"CustomStyle", "", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("Nome7", "Desc3","CustomStyle", "", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("Nome8", null,"CustomStyle", "", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services));
    }

    public void AddQuestionBtn() {
        //TODO: show add question dialog
        _services.GetRequiredService<WindowNotificationManager>()
            .Show(new Notification("Not implemented", "Question creation dialog not implemented yet!"));
    }

    public void DeleteQuestion(object param) {
        var question = param as SingleQuestionViewModel;
        var deletedIndex = question!.Index;
        
        question.OnRemove();
        Questions.Remove(question);
        
        foreach (var q in Questions)
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