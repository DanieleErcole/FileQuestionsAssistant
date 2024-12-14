using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.FileHandling;
using Core.Questions;
using Core.Questions.Word;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms;

public partial class CreateStyleQuestionFormViewModel(IServiceProvider services) : QuestionFormBaseVM(services) {
    
    public static string[] Alignments = ["left", "center", "right"]; 
    
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
            
            stream.Position = 0;
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

    public override AbstractQuestion? CreateQuestion() {
        if (string.IsNullOrWhiteSpace(Name) || _ogFile is null || string.IsNullOrWhiteSpace(Path) || string.IsNullOrWhiteSpace(StyleName)) {
            ErrorMsg = Lang.Lang.MissingRequiredFields;
            return null;
        }

        Color? c = Color is { } color ? System.Drawing.Color.FromArgb((int) color.ToUInt32()) : null;
        return new CreateStyleQuestion(Path, Name, Desc, _ogFile, StyleName, BasedOnSelected, 
            FontNamesSelected, _fontSize == 0 ? null : _fontSize, c, AlignmentSelected);
    }
    
}