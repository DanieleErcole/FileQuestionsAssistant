using System;
using System.Collections.Generic;
using Core.FileHandling;
using DynamicData.Binding;
using ReactiveUI;

namespace UI.ViewModels.Questions;

public abstract class SingleQuestionViewModel : ViewModelBase {
    
    protected IServiceProvider _services;
    
    public int Index { get; set; }
    public bool IsSelected { get; set; }

    public abstract string Name { get; }
    public abstract string Description { get; }
    
    protected List<IFile> Files { get; } = [];
    public string FileCount => Files.Count + " " + Lang.Localization.FilesSelected;

    protected SingleQuestionViewModel(IServiceProvider services) {
        _services = services;
    }

    public abstract void UploadFiles();

}