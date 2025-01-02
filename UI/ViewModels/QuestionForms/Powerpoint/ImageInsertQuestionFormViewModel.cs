using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Questions;
using Core.Questions.Powerpoint;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms.Powerpoint;

public partial class ImageInsertQuestionFormViewModel : QuestionFormBaseVM {

    protected override FilePickerFileType FileType => FileTypesHelper.Powerpoint;
    
    public string[] Origins { get; } = [Lang.Lang.OriginTopLeftCornerText, Lang.Lang.OriginCenterText];
    
    [ObservableProperty]
    private double? _x;
    [ObservableProperty]
    private double? _y;
    [ObservableProperty]
    private double? _width;
    [ObservableProperty]
    private double? _height;
    [ObservableProperty]
    private Origin _vOrigin = Origin.TopLeftCorner;
    [ObservableProperty]
    private Origin _hOrigin = Origin.TopLeftCorner;
    
    public ImageInsertQuestionFormViewModel(IErrorHandlerService errorHandler, IStorageService storageService, AbstractQuestion? q = null) 
        : base(errorHandler, storageService, q) {
        if (q is null) return;

        X = q.Params.Get<double?>("x");
        Y = q.Params.Get<double?>("y");
        Width = q.Params.Get<double?>("width");
        Height = q.Params.Get<double?>("height");
        HOrigin = q.Params.Get<Origin>("hOrigin");
        VOrigin = q.Params.Get<Origin>("vOrigin");
    }

    public override AbstractQuestion? CreateQuestion() {
        if (string.IsNullOrWhiteSpace(Name) || OgFile is null || string.IsNullOrWhiteSpace(Path)) {
            ErrorMsg = Lang.Lang.MissingRequiredFields;
            return null;
        }
        if (X is null && Y is null && Width is null && Height is null) {
            ErrorMsg = Lang.Lang.NoParametersSetText;
            return null;
        }
        
        Desc = string.IsNullOrWhiteSpace(Desc) ? null : Desc;
        return new ImageInsertQuestion(Path, Name, Desc, OgFile, X, Y, Width, Height, VOrigin, HOrigin);
    }
    
}