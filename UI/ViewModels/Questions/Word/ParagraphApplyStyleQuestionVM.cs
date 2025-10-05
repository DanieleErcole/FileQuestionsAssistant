using System.Collections.Generic;
using System.Linq;
using Core.Evaluation;
using Core.Questions.Word;

namespace UI.ViewModels.Questions.Word;

public class ParagraphApplyStyleQuestionVM(ParagraphApplyStyleQuestion q) : WordQuestionViewModel(q) {
    
    public override string Description => Question.Desc ?? Lang.Lang.ParagraphApplyStyleQuestionDesc;
    
    public override Dictionary<string, object?> GetLocalizedQuestionParams() => new() {
        [Lang.Lang.StyleNameLabel] = q.StyleName
    };

    protected override List<Dictionary<string, (object?, bool?)>> GetLocalizedResultParams(Result res) {
        return res.EachParamsWithRes().Select(d => new Dictionary<string, (object?, bool?)> {
            [Lang.Lang.StyleNameLabel] = d["styleName"],
        }).ToList();
    }
    
}