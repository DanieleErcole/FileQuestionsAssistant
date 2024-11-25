using System;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Word;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels.Questions;

public class CreateStyleQuestionVM : WordQuestionViewModel {
    
    public override string Name => Lang.Lang.CreateStyleQuestionName;
    public override string Description => Lang.Lang.CreateStyleQuestionDesc;

    public CreateStyleQuestionVM(CreateStyleQuestion question, IServiceProvider services) : base(services) {
        var e = _services.GetRequiredService<Evaluator<WordFile>>();
        e.AddQuestion(question);
        Index = e.Questions.IndexOf(question);
    }
    
}