using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;
using UI.Services;

namespace UI.ViewModels.Questions;

public static class QuestionExtensions {
    public static SingleQuestionViewModel ToViewModel(this IQuestion q, Evaluator evaluator, ErrorHandler errorHandler, IStorageProvider storageProvider) {
        return q switch {
            CreateStyleQuestion csq => new CreateStyleQuestionVM(csq, evaluator, errorHandler, storageProvider),
            ParagraphApplyStyleQuestion par => new ParagraphApplyStyleQuestionVM(par, evaluator, errorHandler, storageProvider),
        };
    }
}

public abstract class SingleQuestionViewModel(AbstractQuestion q, Evaluator evaluator, ErrorHandler errorHandler, IStorageProvider storageProvider) : ViewModelBase {
    
    protected readonly Evaluator Evaluator = evaluator;
    protected readonly ErrorHandler ErrorHandler = errorHandler;
    protected readonly IStorageProvider StorageProvider = storageProvider;

    public AbstractQuestion Question { get; } = q;
    public string Name => Question.Name;
    public abstract string Description { get; }
    public string Path => Question.Path;
    public abstract string Icon { get; }

    public abstract Task AddFiles();
    public abstract Dictionary<string, object?> GetLocalizedQuestionParams();
    public abstract List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res);

}