using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;
using UI.Services;

namespace UI.ViewModels.Questions;

public class ParagraphApplyStyleQuestionVM(ParagraphApplyStyleQuestion q, Evaluator evaluator, ErrorHandler errorHandler, IStorageProvider storageProvider) 
    : WordQuestionViewModel(q, evaluator, errorHandler, storageProvider) {
    
    public override string Description => Question.Desc ?? Lang.Lang.ParagraphApplyStyleQuestionDesc;
    
    public override Dictionary<string, object?> GetLocalizedQuestionParams() {
        var qParams = (Question as ParagraphApplyStyleQuestion)!.Params;
        return new Dictionary<string, object?> {
            [Lang.Lang.StyleNameLabel] = qParams.Get<string>("styleName"),
        };
    }

    public override List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res) {
        return res.EachParamsWithRes().Select(d => new Dictionary<string, (object?, bool)> {
            [Lang.Lang.StyleNameLabel] = d["styleName"],
        }).ToList();
    }
    
}