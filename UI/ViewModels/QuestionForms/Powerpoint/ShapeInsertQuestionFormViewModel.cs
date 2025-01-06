using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Questions;
using Core.Questions.Powerpoint;
using Core.Utils;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms.Powerpoint;

public partial class ShapeInsertQuestionFormViewModel : QuestionFormBaseVM {

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
    private int? _rotation;
    [ObservableProperty]
    private Origin _vOrigin = Origin.TopLeftCorner;
    [ObservableProperty]
    private Origin _hOrigin = Origin.TopLeftCorner;
    [ObservableProperty]
    private bool _flipH;
    [ObservableProperty]
    private bool _flipV;
    
    public ShapeInsertQuestionFormViewModel(IErrorHandlerService errorHandler, IStorageService storageService, AbstractQuestion? q = null) 
        : base(errorHandler, storageService, q) {
        if (q is null) return;

        X = q.Params.Get<double?>("x");
        Y = q.Params.Get<double?>("y");
        Width = q.Params.Get<double?>("width");
        Height = q.Params.Get<double?>("height");
        Rotation = q.Params.Get<int?>("rotation");
        HOrigin = q.Params.Get<Origin>("hOrigin");
        VOrigin = q.Params.Get<Origin>("vOrigin");
        FlipH = q.Params.Get<bool>("flipH");
        FlipV = q.Params.Get<bool>("flipV");
    }

    public override AbstractQuestion? CreateQuestion() {
        if (string.IsNullOrWhiteSpace(Name) || OgFile is null || string.IsNullOrWhiteSpace(Path)) {
            ErrorMsg = Lang.Lang.MissingRequiredFields;
            return null;
        }
        if (X is null && Y is null && Width is null && Height is null && Rotation is null) {
            ErrorMsg = Lang.Lang.NoParametersSetText;
            return null;
        }
        
        Rotation = Rotation == 0 ? null : Rotation;
        Desc = string.IsNullOrWhiteSpace(Desc) ? null : Desc;
        return new ShapeInsertQuestion(Path, Name, Desc, OgFile, X, Y, Width, Height, Rotation, VOrigin, HOrigin, FlipV, FlipH);
    }
    
}