using System;
using System.Collections.Generic;
using System.Linq;
using Core.Evaluation;
using Core.FileHandling;
using UI.Utils;

namespace UI.ViewModels.Questions;

public class FileResultViewModel : ViewModelBase {
    
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
    
    public bool IsSuccess => Result is { IsSuccessful: true };
    public bool IsFailed => Result is { IsSuccessful: false };
    public bool IsExpandable => Result is not null && Result.EachParamsWithRes().Any();
    public List<TreeViewNode> FileParams => Result is not null ? _vm.ResultParamsAsNodes(Result) : [];

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