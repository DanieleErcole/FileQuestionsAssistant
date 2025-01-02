using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.FileHandling;
using Core.Questions;
using Core.Questions.Word;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms.Word;

public partial class CreateStyleQuestionFormViewModel : QuestionFormBaseVM {
    
    public static readonly string[] Alignments = ["left", "center", "right"];

    protected override FilePickerFileType FileType => FileTypesHelper.Word;
    
    public string? FontNamesSelected { get; set; }
    
    public string? AlignmentSelected { get; set; }
    public string? BasedOnSelected { get; set; }

    #region Observable properties
    
    [ObservableProperty]
    private string? _styleName;
    
    [ObservableProperty]
    private ObservableCollection<string> _basedOnStyles = [];
    
    [ObservableProperty]
    private ObservableCollection<string> _fontNames = [];
    
    [ObservableProperty]
    private Avalonia.Media.Color? _color;
    
    private int _fontSize;
    public string FontSize {
        get => _fontSize.ToString();
        set {
            try {
                _fontSize = int.Parse(value);
            } catch (Exception _) {
                _fontSize = 0;
            }
            OnPropertyChanged(nameof(_fontSize));
        }
    }

    #endregion

    public CreateStyleQuestionFormViewModel(IErrorHandlerService errorHandler, IStorageService storageService, AbstractQuestion? q = null) 
        : base(errorHandler, storageService, q) {
        if (q is null) return;
        
        StyleName = q.Params.Get<string>("styleName");
        Color = q.Params.Get<Color?>("color") is { } color ? Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B) : null;
        FontSize = q.Params.Get<int?>("fontSize") is {} n ? n.ToString() : 0.ToString();
        
        using WordFile file = OgFile!;
        BasedOnStyles = new ObservableCollection<string>(
            file.Styles
                .Where(s => s.Type?.InnerText == "paragraph" && s.StyleName is not null)
                .Select(s => s.StyleName!.Val!.Value!)
        );
        FontNames = new ObservableCollection<string>(file.Fonts.Select(s => s.Name!.Value!));
        BasedOnSelected = q.Params.Get<string?>("baseStyleName");
        FontNamesSelected = q.Params.Get<string?>("fontName");
        AlignmentSelected = q.Params.Get<string?>("alignment");
    }

    public override async Task UploadOgFile() {
        try {
            await base.UploadOgFile();
            if (OgFile is null) return;
            using WordFile file = OgFile;
            
            BasedOnStyles = new ObservableCollection<string>(
                file.Styles
                    .Where(s => s.Type?.InnerText == "paragraph" && s.StyleName is not null)
                    .Select(s => s.StyleName!.Val!.Value!)
            );
            FontNames = new ObservableCollection<string>(file.Fonts.Select(s => s.Name!.Value!));
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
    }

    public override AbstractQuestion? CreateQuestion() {
        if (string.IsNullOrWhiteSpace(Name) || OgFile is null || string.IsNullOrWhiteSpace(Path) || string.IsNullOrWhiteSpace(StyleName)) {
            ErrorMsg = Lang.Lang.MissingRequiredFields;
            return null;
        }

        Desc = string.IsNullOrWhiteSpace(Desc) ? null : Desc;
        Color? c = Color is { } color ? System.Drawing.Color.FromArgb((int) color.ToUInt32()) : null;
        return new CreateStyleQuestion(Path, Name, Desc, OgFile, StyleName, BasedOnSelected, 
            FontNamesSelected, _fontSize == 0 ? null : _fontSize, c, AlignmentSelected);
    }
    
}