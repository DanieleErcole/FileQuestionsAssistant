using System.Collections.Generic;
using System.Linq;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Powerpoint;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.Questions.Powerpoint;

public class ImageInsertQuestionVM(ImageInsertQuestion q, Evaluator evaluator, IErrorHandlerService errorHandler, IStorageService storageService) 
    : PowerpointQuestionViewModel(q, evaluator, errorHandler, storageService) {
    
    public override string Description => Question.Desc ?? Lang.Lang.PptxImageInsertQuestionDesc;
    
    public override Dictionary<string, object?> GetLocalizedQuestionParams() {
        var qParams = (Question as ImageInsertQuestion)!.Params;
        return new Dictionary<string, object?> {
            [Lang.Lang.XPosLabel] = qParams.Get<double?>("x")?.ToString() ?? Lang.Lang.AnyText,
            [Lang.Lang.YPosLabel] = qParams.Get<double?>("y")?.ToString()  ?? Lang.Lang.AnyText,
            [Lang.Lang.WidthLabel] = qParams.Get<double?>("width")?.ToString()  ?? Lang.Lang.AnyText,
            [Lang.Lang.HeightLabel] = qParams.Get<double?>("height")?.ToString()  ?? Lang.Lang.AnyText,
            [Lang.Lang.VerticalOriginLabel] = qParams.Get<Origin>("vOrigin").ToFriendlyString(),
            [Lang.Lang.HorizontalOriginLabel] = qParams.Get<Origin>("hOrigin").ToFriendlyString(),
        };
    }

    protected override List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res) =>
        res.EachParamsWithRes().ToList().Select(d => new Dictionary<string, (object?, bool)> {
            ["imageName"] = (d["imageName"].Item1, d["imageName"].Item2),
            [Lang.Lang.XPosLabel] = (d["x"].Item1 ?? Lang.Lang.NoneText, d["x"].Item2),
            [Lang.Lang.YPosLabel] = (d["y"].Item1 ?? Lang.Lang.NoneText, d["y"].Item2),
            [Lang.Lang.WidthLabel] = (d["width"].Item1 ?? Lang.Lang.NoneText, d["width"].Item2),
            [Lang.Lang.HeightLabel] = (d["height"].Item1 ?? Lang.Lang.NoneText, d["height"].Item2),
            [Lang.Lang.VerticalOriginLabel] = ((d["vOrigin"].Item1 as Origin?)?.ToFriendlyString() ?? Lang.Lang.NoneText, d["vOrigin"].Item2),
            [Lang.Lang.HorizontalOriginLabel] = ((d["hOrigin"].Item1 as Origin?)?.ToFriendlyString() ?? Lang.Lang.NoneText, d["hOrigin"].Item2),
        }).ToList();
    
}