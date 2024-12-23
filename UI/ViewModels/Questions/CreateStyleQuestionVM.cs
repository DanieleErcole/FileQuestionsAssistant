using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils;
using UI.Services;

namespace UI.ViewModels.Questions;

public class CreateStyleQuestionVM(CreateStyleQuestion q, Evaluator evaluator, IErrorHandlerService errorHandler, IStorageProvider storageProvider) 
    : WordQuestionViewModel(q, evaluator, errorHandler, storageProvider) {
    
    public override string Description => Question.Desc ?? Lang.Lang.CreateStyleQuestionDesc;

    public override Dictionary<string, object?> GetLocalizedQuestionParams() {
        var qParams = (Question as CreateStyleQuestion)!.Params;
        return new Dictionary<string, object?> {
            [Lang.Lang.StyleNameLabel] = qParams.Get<string>("styleName"),
            [Lang.Lang.BasedOnLabel] = qParams.Get<string?>("baseStyleName") ?? Lang.Lang.AnyText,
            [Lang.Lang.FontNameLabel] = qParams.Get<string?>("fontName") ?? Lang.Lang.AnyText,
            [Lang.Lang.FontSizeLabel] = qParams.Get<int?>("fontSize")?.ToString() ?? Lang.Lang.AnyText,
            [Lang.Lang.ColorLabel] = qParams.Get<Color?>("color")?.ToHexString() ?? Lang.Lang.AnyText,
            [Lang.Lang.AlignmentLabel] = qParams.Get<string?>("alignment") ?? Lang.Lang.AnyText,
        };
    }

    public override List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res) {
        return res.EachParamsWithRes().Select(d => new Dictionary<string, (object?, bool)> {
            [Lang.Lang.StyleNameLabel] = d["styleName"],
            [Lang.Lang.BasedOnLabel] = (d["baseStyleName"].Item1 ?? Lang.Lang.NoneText, d["baseStyleName"].Item2),
            [Lang.Lang.FontNameLabel] = (d["fontName"].Item1 ?? Lang.Lang.NoneText, d["fontName"].Item2),
            [Lang.Lang.FontSizeLabel] = (d["fontSize"].Item1?.ToString() ?? Lang.Lang.NoneText, d["fontSize"].Item2),
            [Lang.Lang.ColorLabel] = (((Color? ) d["color"].Item1)?.ToHexString() ?? Lang.Lang.NoneText, d["color"].Item2),
            [Lang.Lang.AlignmentLabel] = (d["alignment"].Item1 ?? Lang.Lang.NoneText, d["alignment"].Item2),
        }).ToList();
    }
    
}