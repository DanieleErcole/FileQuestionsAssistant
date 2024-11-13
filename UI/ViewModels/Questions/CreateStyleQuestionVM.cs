using System;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Word;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels.Questions;

public class CreateStyleQuestionVM : SingleQuestionViewModel {
    
    public override string Name => Lang.Localization.CreateStyleQuestionName;
    public override string Description => Lang.Localization.CreateStyleQuestionDesc;

    public CreateStyleQuestionVM(CreateStyleQuestion question, IServiceProvider services) : base(services) {
        _services.GetRequiredService<Evaluator<WordFile>>().AddQuestion(question);
    }
    
    public override void UploadFiles() {
        var e = _services.GetRequiredService<Evaluator<WordFile>>();
        // e.SetFiles(Id, );
    }
    
}