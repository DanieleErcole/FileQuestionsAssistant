using System;
using Avalonia.Platform.Storage;
using ReactiveUI;

namespace UI.ViewModels.Questions;

public abstract class SingleQuestionViewModel : ViewModelBase {
    
    protected IServiceProvider _services;
    
    public int Index { get; set; }

    private bool _selected;
    public bool IsSelected {
        get => _selected; 
        set => this.RaiseAndSetIfChanged(ref _selected, value); 
    }

    public abstract string Name { get; }
    public abstract string Description { get; }
    
    protected abstract FilePickerFileType FileType { get; }

    private string _fileCount = "0";
    public string FileCount {
        get => _fileCount + " " + Lang.Lang.FilesSelected;
        set => this.RaiseAndSetIfChanged(ref _fileCount, value);
    }

    protected SingleQuestionViewModel(IServiceProvider services) {
        _services = services;
    }

    public abstract void UploadFiles();

}