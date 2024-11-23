using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
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
    
    private bool _clearBtnVisible;
    public bool ClearBtnVisible {
        get => _clearBtnVisible;
        set => this.RaiseAndSetIfChanged(ref _clearBtnVisible, value);
    }

    public string Icon {
        get {
            if (FileType.Patterns!.Contains("*.docx"))
                return "/Assets/docx.svg";
            return FileType.Patterns!.Contains("*.xlsx") ? "/Assets/xlsx.svg" : "/Assets/pptx.svg";
        }
    }

    private string _fileCount = "0";
    public string FileCount {
        get => _fileCount + " " + Lang.Lang.FilesSelected;
        set => this.RaiseAndSetIfChanged(ref _fileCount, value);
    }

    protected SingleQuestionViewModel(IServiceProvider services) {
        _services = services;
        this.WhenAnyValue(x => x.FileCount)
            .Subscribe(x => ClearBtnVisible = int.Parse(x.Split(" ")[0]) > 0);
    }

    protected Task<IReadOnlyList<IStorageFile>> OpenFiles() {
        return _services.GetRequiredService<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions {
            AllowMultiple = true,
            FileTypeFilter = new[] { FileType }
        });
    }

    public abstract void UploadFiles();
    public abstract void ClearFiles();

}