using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils;

namespace UI.ViewModels.Questions;

public class CreateStyleQuestionVM : WordQuestionViewModel {
    public CreateStyleQuestionVM(CreateStyleQuestion question, IServiceProvider services) 
        : base(question.Name, question.Desc ?? Lang.Lang.CreateStyleQuestionDesc, services) {
        Index = _services.Get<Evaluator>().Questions.IndexOf(question);
    }

    public override Dictionary<string, object?> GetLocalizedQuestionParams() {
        var qParams = (_services.Get<Evaluator>().Questions[Index] as CreateStyleQuestion)!.Params;
        return new Dictionary<string, object?> {
            [Lang.Lang.StyleNameLabel] = qParams.Get<string>("styleName"),
            [Lang.Lang.BasedOnLabel] = qParams.Get<string?>("baseStyleName") ?? Lang.Lang.AnyText,
            [Lang.Lang.FontNameLabel] = qParams.Get<string?>("fontName") ?? Lang.Lang.AnyText,
            [Lang.Lang.FontSizeLabel] = qParams.Get<int?>("fontSize")?.ToString() ?? Lang.Lang.AnyText,
            [Lang.Lang.ColorLabel] = qParams.Get<Color?>("color")?.ToHexString() ?? Lang.Lang.AnyText,
            [Lang.Lang.AlignmentLabel] = qParams.Get<string?>("alignment") ?? Lang.Lang.AnyText,
        };
    }
}