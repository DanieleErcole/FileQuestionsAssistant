using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;

namespace UI.ViewModels.Questions;

public static class QuestionExtensions {
    public static SingleQuestionViewModel ToViewModel(this IQuestion q, IServiceProvider services) {
        return q switch {
            CreateStyleQuestion csq => new CreateStyleQuestionVM(csq, services)
        };
    }
}

public abstract class SingleQuestionViewModel(AbstractQuestion q, IServiceProvider services) : ViewModelBase {
    
    protected readonly IServiceProvider _services = services;

    public AbstractQuestion Question { get; } = q;
    public string Name => Question.Name;
    public abstract string Description { get; }
    public string Path => Question.Path;
    public abstract string Icon { get; }

    public abstract Task AddFiles();
    public abstract Dictionary<string, object?> GetLocalizedQuestionParams();
    public abstract List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res);

}