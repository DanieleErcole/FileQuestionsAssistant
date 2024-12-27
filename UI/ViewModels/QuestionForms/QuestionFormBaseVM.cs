using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.FileHandling;
using Core.Questions;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms;

public abstract partial class QuestionFormBaseVM : ViewModelBase {
    
    public static readonly Func<double, string> IntFormat = input => Math.Max(0, (int) input).ToString();

    protected readonly IErrorHandlerService ErrorHandler;
    private readonly IStorageService _storageService;
    
    private string? _errorMsg;
    public string? ErrorMsg {
        get => _errorMsg;
        set {
            SetProperty(ref _errorMsg, value);
            OnPropertyChanged(nameof(IsError));
        }
    }
    public bool IsError => ErrorMsg is not null;

    protected byte[]? OgFile;
    
    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private string? _desc;
    
    [ObservableProperty]
    private string _filename = Lang.Lang.NoFilesSelected;
    
    [ObservableProperty]
    private string? _path;

    protected QuestionFormBaseVM(IErrorHandlerService errorHandler, IStorageService storageService, AbstractQuestion? q = null) {
        ErrorHandler = errorHandler;
        _storageService = storageService;
        if (q is null) return;
        
        OgFile = q.OgFile;
        Name = q.Name;
        Desc = q.Desc;
        Path = q.Path;
    }
    
    public void CloseErr() {
        ErrorMsg = null;
    }

    public virtual async Task UploadOgFile() {
        var files = await _storageService.GetFilesAsync(new FilePickerOpenOptions {
            AllowMultiple = false,
            FileTypeFilter = [FileTypesHelper.Word],
        });
        
        if (files.Count == 0)
            return;
        
        using var f = files[0];
        if ((await f.GetBasicPropertiesAsync()).Size > IFile.MaxBytesFileSize)
            throw new FileTooLarge();
        
        await using var stream = await f.OpenReadAsync();
        using var memStream = new MemoryStream();
        await stream.CopyToAsync(memStream);
            
        OgFile = memStream.ToArray();
        Filename = f.Name;
    }

    public async Task LoadLocation() {
        using var file = await _storageService.SaveFileAsync(new FilePickerSaveOptions {
            FileTypeChoices = [QuestionSerializer.FileType],
        });
        if (file is null) return;
        Path = Uri.UnescapeDataString(file.Path.AbsolutePath);
    }
    
    public abstract AbstractQuestion? CreateQuestion();

}