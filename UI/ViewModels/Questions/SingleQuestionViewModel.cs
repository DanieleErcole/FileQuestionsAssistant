using System;
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

public abstract class SingleQuestionViewModel(string name, string desc, IServiceProvider services) : ViewModelBase {
    
    protected readonly IServiceProvider _services = services;
    
    public int Index { get; set; }
    public string Name { get; } = name;
    public string Description { get; } = desc;
    public string Path => _services.Get<Evaluator>().Questions[Index].Path;
    public abstract string Icon { get; }

    public abstract Task UploadFiles();

}