using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Word;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
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

    private string _totFileStr = "0";
    public string TotFileCountStr {
        get => $"{_totFileStr} {Lang.Lang.FilesSelected}";
        private set => this.RaiseAndSetIfChanged(ref _totFileStr, value);
    }

    private bool? _selectionState;
    public bool? SelectionState {
        get => _selectionState;
        private set => this.RaiseAndSetIfChanged(ref _selectionState, value);
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
        
        SetupReactiveComponents();

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
        DeleteSelectionCommand = ReactiveCommand.Create(() => Questions.RemoveMany(Questions.Where(q => q.IsSelected)), isAnySelected);
        EditQuestionCommand = ReactiveCommand.Create(() => {
            throw new NotImplementedException();  
        }, isOnlyOneSelected);
    }

    private void SetupReactiveComponents() {
        Questions.ToObservableChangeSet()
            .AutoRefresh(q => q.FileCount)
            .ToCollection()
            .Subscribe(questions => {
                TotFileCountStr = questions.Sum(q => int.Parse(q.FileCount.Split(" ")[0])).ToString();
            });

        Questions.ToObservableChangeSet()
            .AutoRefresh(q => q.IsSelected)
            .ToCollection()
            .Subscribe(questions => {
                var count = questions.Count(q => q.IsSelected);
                if (count == questions.Count)
                    SelectionState = true;
                else if (count == 0)
                    SelectionState = false;
                else SelectionState = null;
            });
    }

    public void HeaderCheckboxToggle() {
        switch (SelectionState) {
            case null or true: {
                foreach (var q in Questions)
                    q.IsSelected = false;
                break;
            }
            case false: {
                foreach (var q in Questions)
                    q.IsSelected = true;
                break;
            }
        }
    }
    
}