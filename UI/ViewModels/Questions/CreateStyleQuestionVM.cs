using System;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels.Questions;

public class CreateStyleQuestionVM : WordQuestionViewModel {
    public CreateStyleQuestionVM(CreateStyleQuestion question, IServiceProvider services) 
        : base(question.Name, question.Desc ?? Lang.Lang.CreateStyleQuestionDesc, services) {
        Index = _services.GetRequiredService<Evaluator>().Questions.IndexOf(question);
    }
}