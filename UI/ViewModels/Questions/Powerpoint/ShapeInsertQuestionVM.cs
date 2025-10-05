using System.Collections.Generic;
using System.Linq;
using Core.Evaluation;
using Core.Questions.Powerpoint;
using Core.Utils.Converters;
using UI.Utils.Converters;

namespace UI.ViewModels.Questions.Powerpoint;

public class ShapeInsertQuestionVM(ShapeInsertQuestion q) : PowerpointQuestionViewModel(q) {
    
    public override string Description => Question.Desc ?? Lang.Lang.PptxShapeInsertQuestionDesc;
    
    public override Dictionary<string, object?> GetLocalizedQuestionParams() => new() {
        [Lang.Lang.XPosLabel] = q.X,
        [Lang.Lang.YPosLabel] = q.Y,
        [Lang.Lang.WidthLabel] = q.Width,
        [Lang.Lang.HeightLabel] = q.Height,
        [Lang.Lang.RotationLabel] = DegreesHelper.ToDegreeString(q.Rotation),
        [Lang.Lang.VerticalOriginLabel] = q.VOrigin,
        [Lang.Lang.HorizontalOriginLabel] = q.HOrigin,
        [Lang.Lang.FlipHLabel] = q.FlipH.ToFriendlyString(),
        [Lang.Lang.FlipVLabel] = q.FlipV.ToFriendlyString(),
    };

    protected override List<Dictionary<string, (object?, bool?)>> GetLocalizedResultParams(Result res) =>
        res.EachParamsWithRes().Select(d => new Dictionary<string, (object?, bool?)> {
            ["name"] = (d["name"].Item1, null),
            [Lang.Lang.XPosLabel] = (d["x"].Item1, d["x"].Item2),
            [Lang.Lang.YPosLabel] = (d["y"].Item1, d["y"].Item2),
            [Lang.Lang.WidthLabel] = (d["width"].Item1, d["width"].Item2),
            [Lang.Lang.HeightLabel] = (d["height"].Item1, d["height"].Item2),
            [Lang.Lang.FlipHLabel] = (((bool) d["flipH"].Item1!).ToFriendlyString(), d["flipH"].Item2),
            [Lang.Lang.FlipVLabel] = (((bool) d["flipV"].Item1!).ToFriendlyString(), d["flipV"].Item2),
            [Lang.Lang.RotationLabel] = (DegreesHelper.ToDegreeString(d["rotation"].Item1 as int?), d["rotation"].Item2),
        }).ToList();
    
}