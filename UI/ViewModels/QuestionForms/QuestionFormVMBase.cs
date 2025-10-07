using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.FileHandling;
using Core.Questions;
using Core.Utils;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms;

public abstract partial class QuestionFormVMBase : ViewModelBase {

    protected readonly IErrorHandlerService ErrorHandler;
    private readonly IStorageService _storageService;

    protected abstract FilePickerFileType FileType { get; }

    [ObservableProperty] 
    private MemoryFile? _ogFile;

    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private string? _desc;
    
    [ObservableProperty]
    private string? _path;

    protected QuestionFormVMBase(IErrorHandlerService errorHandler, IStorageService storageService, AbstractQuestion? q = null) {
        ErrorHandler = errorHandler;
        _storageService = storageService;
        if (q is null) return;
        
        OgFile = q.OgFile;
        Name = q.Name;
        Desc = q.Desc;
        Path = q.Path;
    }

    public virtual async Task UploadOgFile() {
        var files = await _storageService.GetFilesAsync(new FilePickerOpenOptions {
            AllowMultiple = false,
            FileTypeFilter = [FileType],
        });
        
        if (files.Count == 0)
            return;
        
        using var f = files[0];
        if ((await f.GetBasicPropertiesAsync()).Size > IFile.MaxBytesFileSize)
            throw new FileTooLarge();
        
        await using var stream = await f.OpenReadAsync();
        using var memStream = new MemoryStream();
        await stream.CopyToAsync(memStream);
        
        OgFile = new MemoryFile(f.Name, memStream.ToArray());
    }

    public async Task LoadLocation() {
        using var file = await _storageService.SaveFileAsync(new FilePickerSaveOptions {
            FileTypeChoices = [QuestionSerializer.FileType],
        });
        if (file is null) return;
        Path = Uri.UnescapeDataString(file.Path.AbsolutePath);
    }
    
    public abstract AbstractQuestion CreateQuestion();

}