using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.FileHandling;
using UI.Utils;

namespace UI.ViewModels.Questions;

public partial class FileResultViewModel(SingleQuestionViewModel vm, IFile file, Result? result, EventHandler fileSelected) : ViewModelBase {
    
    public IFile File { get; } = file;
    public Result? Result { get; } = result;

    private bool _isSelected;
    public bool IsSelected {
        get => _isSelected;
        set {
            SetProperty(ref _isSelected, value);
            fileSelected.Invoke(this, EventArgs.Empty);
        }
    }
    
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