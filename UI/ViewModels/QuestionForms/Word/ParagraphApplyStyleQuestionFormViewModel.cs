using System;
using System.Collections.ObjectModel;
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

public partial class ParagraphApplyStyleQuestionFormViewModel : QuestionFormVMBase {
    
    protected override FilePickerFileType FileType => FileTypesHelper.Word;
    
    [ObservableProperty]
    private ObservableCollection<string> _styleNames = [];
    
    public string? StyleNameSelected { get; set; }
    
    public ParagraphApplyStyleQuestionFormViewModel(IErrorHandlerService errHandler, IStorageService storageService, ParagraphApplyStyleQuestion? q = null) 
        : base(errHandler, storageService, q) {
        if (q is null) return;
        
        using WordFile file = OgFile!;
        StyleNames = new ObservableCollection<string>(
            file.Styles
                .Where(s => s.Type?.InnerText == "paragraph" && s.StyleName is not null)
                .Select(s => s.StyleName!.Val!.Value!)
        );
        StyleNameSelected = q.StyleName;
    }
    
    public override async Task UploadOgFile() {
        try {
            await base.UploadOgFile();
            if (OgFile is null) return;
            using WordFile file = OgFile;
            
            StyleNames = new ObservableCollection<string>(
                file.Styles
                    .Where(s => s.Type?.InnerText == "paragraph" && s.StyleName is not null)
                    .Select(s => s.StyleName!.Val!.Value!)
            );
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
    }
    
    public override AbstractQuestion CreateQuestion() {
        if (string.IsNullOrWhiteSpace(Name) || OgFile is null || string.IsNullOrWhiteSpace(Path) || string.IsNullOrWhiteSpace(StyleNameSelected))
            throw new FormError(Lang.Lang.MissingRequiredFields);

        Desc = string.IsNullOrWhiteSpace(Desc) ? null : Desc;
        return new ParagraphApplyStyleQuestion(Path, Name, Desc, OgFile, StyleNameSelected);
    }
    
}