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

namespace UI.ViewModels.QuestionForms;

public partial class ParagraphApplyStyleQuestionFormViewModel : QuestionFormBaseVM {
    
    [ObservableProperty]
    private ObservableCollection<string> _styleNames = [];
    
    public string? StyleNameSelected { get; set; }
    
    public ParagraphApplyStyleQuestionFormViewModel(IErrorHandlerService errHandler, IStorageProvider storageProvider, AbstractQuestion? q = null) 
        : base(errHandler, storageProvider, q) {
        if (q is null) return;
        
        Filename = "Original file";
        using WordFile file = _ogFile!;
        StyleNames = new ObservableCollection<string>(
            file.Styles
                .Where(s => s.Type?.InnerText == "paragraph" && s.StyleName is not null)
                .Select(s => s.StyleName!.Val!.Value!)
        );
        StyleNameSelected = q.Params.Get<string>("styleName");
    }
    
    public override async Task UploadOgFile() {
        try {
            await base.UploadOgFile();
            if (_ogFile is null) return;
            using WordFile file = _ogFile;
            
            StyleNames = new ObservableCollection<string>(
                file.Styles
                    .Where(s => s.Type?.InnerText == "paragraph" && s.StyleName is not null)
                    .Select(s => s.StyleName!.Val!.Value!)
            );
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
    }
    
    public override AbstractQuestion? CreateQuestion() {
        if (string.IsNullOrWhiteSpace(Name) || _ogFile is null || string.IsNullOrWhiteSpace(Path) || string.IsNullOrWhiteSpace(StyleNameSelected)) {
            ErrorMsg = Lang.Lang.MissingRequiredFields;
            return null;
        }
        return new ParagraphApplyStyleQuestion(Path, Name, Desc, _ogFile, StyleNameSelected);
    }
    
}