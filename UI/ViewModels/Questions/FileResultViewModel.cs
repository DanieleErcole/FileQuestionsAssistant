using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.FileHandling;
using UI.Utils;

namespace UI.ViewModels.Questions;

public partial class FileResultViewModel(SingleQuestionViewModel vm, IFile file, Result? result) : ViewModelBase {
    
    public IFile File { get; } = file;
    public Result? Result { get; } = result;

    [ObservableProperty]
    private bool _isSelected;
    [ObservableProperty]
    private bool _isExpanded;

    public bool IsSuccess => Result is { IsSuccessful: true };
    public bool IsFailed => Result is { IsSuccessful: false };
    public bool IsExpandable => Result is not null && Result.EachParamsWithRes().Any();
    public List<TreeViewNode> FileParams => Result is not null ? vm.ResultParamsAsNodes(Result) : [];

    public string Icon => Result switch {
        not null => Result.IsSuccessful ? "Checkmark" : "Dismiss",
        _ => "Document"
    };
    
}