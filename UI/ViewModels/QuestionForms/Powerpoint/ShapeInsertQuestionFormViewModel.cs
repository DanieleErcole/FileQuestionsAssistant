using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Questions;
using Core.Questions.Powerpoint;
using Core.Utils;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.QuestionForms.Powerpoint;

public partial class ShapeInsertQuestionFormViewModel : QuestionFormVMBase {

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
    
    public ShapeInsertQuestionFormViewModel(IErrorHandlerService errorHandler, IStorageService storageService, ShapeInsertQuestion? q = null) 
        : base(errorHandler, storageService, q) {
        if (q is null) return;

        X = q.X;
        Y = q.Y;
        Width = q.Width;
        Height = q.Height;
        Rotation = q.Rotation;
        HOrigin = q.HOrigin;
        VOrigin = q.VOrigin;
        FlipH = q.FlipH;
        FlipV = q.FlipV;
    }

    public override AbstractQuestion CreateQuestion() {
        if (string.IsNullOrWhiteSpace(Name) || OgFile is null || string.IsNullOrWhiteSpace(Path))
            throw new FormError(Lang.Lang.MissingRequiredFields);
        if (X is null && Y is null && Width is null && Height is null && Rotation is null)
            throw new FormError(Lang.Lang.NoParametersSetText);
        
        Rotation = Rotation == 0 ? null : Rotation;
        Desc = string.IsNullOrWhiteSpace(Desc) ? null : Desc;
        return new ShapeInsertQuestion(Path, Name, Desc, OgFile, X, Y, Width, Height, Rotation, VOrigin, HOrigin, FlipV, FlipH);
    }
    
}