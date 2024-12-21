using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.Questions;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms;

public abstract partial class QuestionFormBaseVM : ViewModelBase {
    
    public static readonly Func<double, string> IntFormat = input => Math.Max(0, (int) input).ToString();

    protected IServiceProvider _services;
    
    private string? _errorMsg;
    public string? ErrorMsg {
        get => _errorMsg;
        set {
            SetProperty(ref _errorMsg, value);
            OnPropertyChanged(nameof(IsError));
        }
    }
    public bool IsError => ErrorMsg is not null;

    protected byte[]? _ogFile;
    
    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private string? _desc;
    
    [ObservableProperty]
    private string _filename = Lang.Lang.NoFilesSelected;
    
    [ObservableProperty]
    private string? _path;

    public QuestionFormBaseVM(IServiceProvider services, AbstractQuestion? q = null) {
        _services = services;
        if (q is null) return;
        
        _ogFile = q.OgFile;
        Name = q.Name;
        Desc = q.Desc;
        Path = q.Path;
    }
    
    public void CloseErr() {
        ErrorMsg = null;
    }

    public virtual async Task UploadOgFile() {
        var files = await _services.Get<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions {
            AllowMultiple = false,
            FileTypeFilter = [FileTypesHelper.Word],
        });
        
        if (files.Count == 0)
            return;
        
        using var f = files[0];
        await using var stream = await f.OpenReadAsync();
        using var memStream = new MemoryStream();
        await stream.CopyToAsync(memStream);
            
        _ogFile = memStream.ToArray();
        Filename = f.Name;
    }

    public async Task LoadLocation() {
        using var file = await _services.Get<IStorageProvider>().SaveFilePickerAsync(new FilePickerSaveOptions {
            FileTypeChoices = [QuestionSerializer.FileType],
        });
        if (file is null) return;
        Path = Uri.UnescapeDataString(file.Path.AbsolutePath);
    }
    
    public abstract AbstractQuestion? CreateQuestion();

}