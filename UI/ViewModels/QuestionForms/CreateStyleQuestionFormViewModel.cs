using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using Core.Questions.Word;
using ReactiveUI;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms;

public class CreateStyleQuestionFormViewModel(IServiceProvider services) : QuestionFormBaseVM(services) {

    private string? _errMsg;
    public string? ErrorMsg {
        get => _errMsg;
        set {
            this.RaiseAndSetIfChanged(ref _errMsg, value);
            this.RaisePropertyChanged(nameof(IsError));
        }
    }
    public bool IsError => ErrorMsg is not null;

    private string _name;
    public string Name { 
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string? _desc;
    public string? Desc { 
        get => _desc;
        set => this.RaiseAndSetIfChanged(ref _desc, value);
    }
    
    private byte[]? _ogFile;
    private string _filename = Lang.Lang.NoFilesSelected;
    public string Filename { 
        get => _filename;
        set => this.RaiseAndSetIfChanged(ref _filename, value);
    }
    
    private string? _path;
    public string Path { 
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }
    
    private string? _styleName;
    public string? StyleName { 
        get => _styleName;
        set => this.RaiseAndSetIfChanged(ref _styleName, value);
    }
    
    private ObservableCollection<string> _basedOnStyles = [];
    public ObservableCollection<string> BasedOnStyles { 
        get => _basedOnStyles;
        set => this.RaiseAndSetIfChanged(ref _basedOnStyles, value);
    }

    public string? BasedOnSelected { get; set; }
    
    private ObservableCollection<string> _fontNames = [];
    public ObservableCollection<string> FontNames { 
        get => _fontNames;
        set => this.RaiseAndSetIfChanged(ref _fontNames, value);
    }
    public string? FontNamesSelected{ get; set; }
    
    public string? AlignmentSelected { get; set; }
    
    private Color? _color;
    public Color? Color { 
        get => _color;
        set => this.RaiseAndSetIfChanged(ref _color, value);
    }
    
    private int _fontSize;
    public string FontSize {
        get => _fontSize.ToString();
        set {
            try {
                var val = int.Parse(value);
                this.RaiseAndSetIfChanged(ref _fontSize, val);
            } catch (Exception _) {
                this.RaiseAndSetIfChanged(ref _fontSize, 0);
            }
        }
    }

    public void CloseErr() {
        ErrorMsg = null;
    }

    public async Task UploadOgFile() {
        var files = await _services.Get<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions {
            Title = "Select the original question file",
            AllowMultiple = false,
            FileTypeFilter = [FileTypesHelper.Word],
        });
        
        if (files.Count == 0)
            return;

        try {
            using var f = files[0];
            await using var stream = await f.OpenReadAsync();
            using var file = new WordFile(f.Name, stream);
        
            BasedOnStyles = new ObservableCollection<string>(
                file.Styles
                    .Where(s => s.Type?.InnerText == "paragraph" && s.StyleName is not null)
                    .Select(s => s.StyleName!.Val!.Value!)
            );
            FontNames = new ObservableCollection<string>(file.Fonts.Select(s => s.Name!.Value!));
            
            using var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            
            _ogFile = memStream.ToArray();
            Filename = file.Name;
        } catch (Exception e) {
            _services.Get<ErrorHandler>().ShowError(e);
        }
    }

    public async Task LoadLocation() {
        using var file = await _services.Get<IStorageProvider>().SaveFilePickerAsync(new FilePickerSaveOptions {
            FileTypeChoices = [QuestionSerializer.FileType],
        });
        if (file is null) return;
        Path = Uri.UnescapeDataString(file.Path.AbsolutePath);
    }

    public override async Task<AbstractQuestion?> CreateQuestion() {
        if (string.IsNullOrWhiteSpace(Name) || _ogFile is null || string.IsNullOrWhiteSpace(Path) || string.IsNullOrWhiteSpace(StyleName)) {
            ErrorMsg = Lang.Lang.MissingRequiredFields;
            return null;
        }
        
        var q = new CreateStyleQuestion(Path, Name, Desc, _ogFile, StyleName, BasedOnSelected, FontNamesSelected, _fontSize == 0 ? null : _fontSize, 
            Color, FontSize);
        if (await _services.Get<QuestionSerializer>().Create(Path, q)) {
            var ev = _services.Get<Evaluator>();
            ev.RemoveQuestion(ev.Questions.FindIndex(x => x.Path == q.Path)); ;
        }
        return q;
    }
    
}