using System;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Word;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels.Questions;

public class CreateStyleQuestionVM : WordQuestionViewModel {

    public CreateStyleQuestionVM(CreateStyleQuestion question, IServiceProvider services) 
        : base(question.Name, question.Desc ?? Lang.Lang.CreateStyleQuestionDesc, services) {
        var e = _services.GetRequiredService<Evaluator<WordFile>>();
        e.AddQuestion(question);
        Index = e.Questions.IndexOf(question);
    }
    
}