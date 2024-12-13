using System.Collections.Generic;
using System.Linq;
using Core.Evaluation;

namespace UI.ViewModels.Questions;

public class FileResultViewModel : ViewModelBase {

    public class Node(string title, object? value, bool res, List<(string, object?, bool)>? sub) {
        public string Title { get; } = title;
        public object? Value { get; } = value;
        public bool IsCorrect { get; } = res;
        public string ResIcon { get; } = res ? "Checkmark" : "Dismiss";
        public List<Node>? SubNodes { get; } = sub?.Select(e => new Node(e.Item1, e.Item2, e.Item3, null)).ToList();
    }
    
    //TODO: review this implementation and error handling during the file upload
    
    public int Index { get; }
    public string Filename { get; }
    public Result? Result { get; }

    private readonly SingleQuestionViewModel _vm;
    
    public bool IsSuccess => Result is { IsSuccessful: true };
    public bool IsFailed => Result is { IsSuccessful: false };
    public List<Node> FileParams => Result is not null ?  _vm
        .GetLocalizedResultParams(Result)
        .Select(d => {
            var first = (d.First().Key, d.First().Value.Item1, d.First().Value.Item2);
            var list = d
                .Select(e => (e.Key, e.Value.Item1, e.Value.Item2))
                .Where(e => e.Item1 != first.Item1 && e.Item1 != first.Item1 && e.Item2 != first.Item2)
                .ToList();
            return new Node(first.Item1, first.Item2, first.Item3, list);
        }).ToList() : [];

    public string Icon => Result switch {
        not null => Result.IsSuccessful ? "Checkmark" : "Dismiss",
        _ => "Document"
    };

    public FileResultViewModel(SingleQuestionViewModel vm, int index, string filename, Result? result) {
        Index = index;
        Filename = filename;
        Result = result;
        _vm = vm;
    }
    
}