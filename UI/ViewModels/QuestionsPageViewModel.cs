using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Core.Questions.Word;
using ReactiveUI;
using UI.ViewModels.Questions;

namespace UI.ViewModels;

public class QuestionsPageViewModel : ViewModelBase {

    private IServiceProvider _services;
    
    private List<SingleQuestionViewModel> _questions = [];
    
    public List<SingleQuestionViewModel> Questions { get => _questions; set => this.RaiseAndSetIfChanged(ref _questions, value); }

    public QuestionsPageViewModel(IServiceProvider services) {
        _services = services;
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("CustomStyle", "Normal", "Consolas", 12, Color.Fuchsia, "center"),
            _services
            ));
    }
    
}