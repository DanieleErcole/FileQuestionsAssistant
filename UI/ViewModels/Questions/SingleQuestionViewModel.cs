using System;
using System.Collections.Generic;
using Core.FileHandling;

namespace UI.ViewModels.Questions;

public abstract class SingleQuestionViewModel : ViewModelBase {
    
    protected IServiceProvider _services;
    
    public int Id { get; set; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    protected List<IFile> Files { get; } = [];
    public int FileCount => Files.Count;

    protected SingleQuestionViewModel(IServiceProvider services) {
        _services = services;
    }

    public abstract void UploadFiles();

}