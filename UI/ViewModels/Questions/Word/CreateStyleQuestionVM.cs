using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Evaluation;
using Core.Questions.Word;
using Core.Utils;

namespace UI.ViewModels.Questions.Word;

public class CreateStyleQuestionVM(CreateStyleQuestion q) : WordQuestionViewModel(q) {
    
    public override string Description => Question.Desc ?? Lang.Lang.CreateStyleQuestionDesc;

    public override Dictionary<string, object?> GetLocalizedQuestionParams() {
        var qParams = (Question as CreateStyleQuestion)!.Params;
        return new Dictionary<string, object?> {
            [Lang.Lang.StyleNameLabel] = qParams.Get<string>("styleName"),
            [Lang.Lang.BasedOnLabel] = qParams.Get<string?>("baseStyleName"),
            [Lang.Lang.FontNameLabel] = qParams.Get<string?>("fontName"),
            [Lang.Lang.FontSizeLabel] = qParams.Get<int?>("fontSize"),
            [Lang.Lang.ColorLabel] = qParams.Get<Color?>("color"),
            [Lang.Lang.AlignmentLabel] = qParams.Get<Alignment?>("alignment"),
        };
    }

    protected override List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res) => 
        res.EachParamsWithRes().Select(d => new Dictionary<string, (object?, bool)> {
            [Lang.Lang.StyleNameLabel] = d["styleName"],
            [Lang.Lang.BasedOnLabel] = (d["baseStyleName"].Item1, d["baseStyleName"].Item2),
            [Lang.Lang.FontNameLabel] = (d["fontName"].Item1, d["fontName"].Item2),
            [Lang.Lang.FontSizeLabel] = (d["fontSize"].Item1, d["fontSize"].Item2),
            [Lang.Lang.ColorLabel] = ((Color? ) d["color"].Item1, d["color"].Item2),
            [Lang.Lang.AlignmentLabel] = (d["alignment"].Item1 as Alignment?, d["alignment"].Item2),
        }).ToList();
    
}