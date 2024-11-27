using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels.Questions;

public abstract class SingleQuestionViewModel : ViewModelBase {
    
    protected readonly IServiceProvider _services;
    
    public int Index { get; set; }
    public string Name { get; }
    public string Description { get; }
    protected abstract FilePickerFileType FileType { get; }

    public string Icon {
        get {
            if (FileType.Patterns!.Contains("*.docx"))
                return "/Assets/docx.svg";
            return FileType.Patterns!.Contains("*.xlsx") ? "/Assets/xlsx.svg" : "/Assets/pptx.svg";
        }
    }

    protected SingleQuestionViewModel(string name, string desc, IServiceProvider services) {
        _services = services;
        Name = name;
        Description = desc;
    }

    protected Task<IReadOnlyList<IStorageFile>> OpenFiles() {
        return _services.GetRequiredService<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions {
            AllowMultiple = true,
            FileTypeFilter = [FileType]
        });
    }
    
    public abstract void OnRemove();
    public abstract void UploadFiles();
    public abstract void ClearFiles();

}