using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Core.Questions.Word;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using UI.ViewModels.Questions;

namespace UI.ViewModels;

public class QuestionsPageViewModel : ViewModelBase {

    private IServiceProvider _services;
    
    private ObservableCollection<SingleQuestionViewModel> _questions = [];
    public ObservableCollection<SingleQuestionViewModel> Questions {
        get => _questions; 
        set => this.RaiseAndSetIfChanged(ref _questions, value);
    }
    
    public ReactiveCommand<Unit, Unit> SaveSelectionCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectionCommand { get; }
    
    public ReactiveCommand<Unit, Unit> EditQuestionCommand { get; }

    public QuestionsPageViewModel(IServiceProvider services) {
        _services = services;
        // Solo per vedere il risultato
        _questions.Add(new CreateStyleQuestionVM(
                new CreateStyleQuestion("CustomStyle", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
                _services
            ));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("CustomStyle", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services
        ));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("CustomStyle", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services
        ));
        _questions.Add(new CreateStyleQuestionVM(
            new CreateStyleQuestion("CustomStyle", "Normal", "Consolas", 12, Color.Fuchsia, "center"), 
            _services
        ));

        var isAnySelected = Questions.ToObservableChangeSet()
            .AutoRefresh(q => q.IsSelected)
            .ToCollection()
            .Select(questions => questions.Any(q => q.IsSelected));
        var isOnlyOneSelected = Questions.ToObservableChangeSet()
            .AutoRefresh(q => q.IsSelected)
            .ToCollection()
            .Select(questions => questions.Count(q => q.IsSelected) == 1);

        SaveSelectionCommand = ReactiveCommand.Create(() => {
            throw new NotImplementedException();
        }, isAnySelected);
        DeleteSelectionCommand = ReactiveCommand.Create(() => {
            throw new NotImplementedException();
        }, isAnySelected);
        EditQuestionCommand = ReactiveCommand.Create(() => {
            throw new NotImplementedException();  
        }, isOnlyOneSelected);
    }
    
}