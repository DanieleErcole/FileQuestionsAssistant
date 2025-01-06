using System.Collections.Generic;
using System.Linq;
using Core.Evaluation;
using Core.Questions.Powerpoint;
using Core.Utils;
using Core.Utils.Converters;
using UI.Services;
using UI.Utils.Converters;

namespace UI.ViewModels.Questions.Powerpoint;

public class ShapeInsertQuestionVM(ShapeInsertQuestion q, Evaluator evaluator, IErrorHandlerService errorHandler, IStorageService storageService) 
    : PowerpointQuestionViewModel(q, evaluator, errorHandler, storageService) {
    
    public override string Description => Question.Desc ?? Lang.Lang.PptxShapeInsertQuestionDesc;
    
    public override Dictionary<string, object?> GetLocalizedQuestionParams() {
        var qParams = (Question as ShapeInsertQuestion)!.Params;
        return new Dictionary<string, object?> {
            [Lang.Lang.XPosLabel] = qParams.Get<double?>("x"),
            [Lang.Lang.YPosLabel] = qParams.Get<double?>("y"),
            [Lang.Lang.WidthLabel] = qParams.Get<double?>("width"),
            [Lang.Lang.HeightLabel] = qParams.Get<double?>("height"),
            [Lang.Lang.RotationLabel] = DegreesHelper.ToDegreeString(qParams.Get<int?>("rotation")),
            [Lang.Lang.VerticalOriginLabel] = qParams.Get<Origin>("vOrigin"),
            [Lang.Lang.HorizontalOriginLabel] = qParams.Get<Origin>("hOrigin"),
            [Lang.Lang.FlipHLabel] = qParams.Get<bool>("flipH").ToFriendlyString(),
            [Lang.Lang.FlipVLabel] = qParams.Get<bool>("flipV").ToFriendlyString(),
        };
    }

    protected override List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res) =>
        res.EachParamsWithRes().Select(d => new Dictionary<string, (object?, bool)> {
            ["name"] = (d["name"].Item1, d["name"].Item2),
            [Lang.Lang.XPosLabel] = (d["x"].Item1, d["x"].Item2),
            [Lang.Lang.YPosLabel] = (d["y"].Item1, d["y"].Item2),
            [Lang.Lang.WidthLabel] = (d["width"].Item1, d["width"].Item2),
            [Lang.Lang.HeightLabel] = (d["height"].Item1, d["height"].Item2),
            [Lang.Lang.FlipHLabel] = (((bool) d["flipH"].Item1!).ToFriendlyString(), d["flipH"].Item2),
            [Lang.Lang.FlipVLabel] = (((bool) d["flipV"].Item1!).ToFriendlyString(), d["flipV"].Item2),
            [Lang.Lang.RotationLabel] = (DegreesHelper.ToDegreeString(d["rotation"].Item1 as int?), d["rotation"].Item2),
            [Lang.Lang.VerticalOriginLabel] = (d["vOrigin"].Item1 as Origin?, d["vOrigin"].Item2),
            [Lang.Lang.HorizontalOriginLabel] = (d["hOrigin"].Item1 as Origin?, d["hOrigin"].Item2),
        }).ToList();
    
}