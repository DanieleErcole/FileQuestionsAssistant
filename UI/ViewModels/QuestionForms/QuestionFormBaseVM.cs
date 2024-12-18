using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Questions;

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
    
    public abstract AbstractQuestion? CreateQuestion();

}