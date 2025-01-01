using System;
using System.Collections.Generic;
using System.Linq;
using Core.Evaluation;
using Core.FileHandling;

namespace UI.ViewModels.Questions;

public partial class FileResultViewModel : ViewModelBase {

    public class Node(string title, object? value, bool res, List<(string, object?, bool)>? sub, bool isFirst = false) {
        public string? Title { get; } = isFirst ? value?.ToString() : title;
        public object? Value { get; } = isFirst ? null : value;
        public bool IsCorrect { get; } = res;
        public string ResIcon { get; } = res ? "Checkmark" : "Dismiss";
        public List<Node>? SubNodes { get; } = sub?.Select(e => new Node(e.Item1, e.Item2, e.Item3, null)).ToList();
    }
    
    private event EventHandler FileSelected; 
    
    public IFile File { get; }
    public Result? Result { get; }
    
    private bool _isSelected;
    public bool IsSelected {
        get => _isSelected;
        set {
            SetProperty(ref _isSelected, value);
            FileSelected.Invoke(this, EventArgs.Empty);
        }
    }

    private readonly SingleQuestionViewModel _vm;

    //TODO: fix the results of the image insert question, I think it's an empty list
    public bool IsSuccess => Result is { IsSuccessful: true };
    public bool IsFailed => Result is { IsSuccessful: false };
    public bool IsExpandable => Result is not null && Result.EachParamsWithRes().Any();
    public List<Node> FileParams => Result is not null ? _vm
        .GetLocalizedResultParams(Result)
        .Select(d => {
            var first = (d.First().Key, d.First().Value.Item1, d.First().Value.Item2);
            var list = d
                .Select(e => (e.Key, e.Value.Item1, e.Value.Item2))
                .Where(e => e.Item1 != first.Item1 && e.Item1 != first.Item1 && e.Item2 != first.Item2)
                .ToList();
            return new Node(first.Item1, first.Item2, first.Item3, list, true);
        }).ToList() : [];

    public string Icon => Result switch {
        not null => Result.IsSuccessful ? "Checkmark" : "Dismiss",
        _ => "Document"
    };

    public FileResultViewModel(SingleQuestionViewModel vm, IFile file, Result? result, EventHandler fileSelected) {
        File = file;
        Result = result;
        _vm = vm;
        FileSelected = fileSelected;
    }
    
}