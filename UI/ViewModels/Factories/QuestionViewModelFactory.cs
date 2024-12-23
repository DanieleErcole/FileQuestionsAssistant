using System;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Word;
using UI.Services;
using UI.ViewModels.QuestionForms;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Factories;

public class QuestionViewModelFactory(Evaluator evaluator, IErrorHandlerService errorHandler, IStorageProvider storageProvider) : IViewModelFactory {
    
    public const int CreateStyleQuestionIndex = 0;
    public const int ParagraphApplyStyleQuestionIndex = 1;
    public const int ImageInsertQuestionIndex = 2;

    public SingleQuestionViewModel NewQuestionVm(IQuestion question) => question switch {
        CreateStyleQuestion csq => new CreateStyleQuestionVM(csq, evaluator, errorHandler, storageProvider),
        ParagraphApplyStyleQuestion par => new ParagraphApplyStyleQuestionVM(par, evaluator, errorHandler, storageProvider),
        _ => throw new ArgumentException("Invalid question type")
    };

    public QuestionFormBaseVM NewQuestionFormVm(int index, AbstractQuestion? question = null) => index switch {
        CreateStyleQuestionIndex => new CreateStyleQuestionFormViewModel(errorHandler, storageProvider, question),
        ParagraphApplyStyleQuestionIndex => new ParagraphApplyStyleQuestionFormViewModel(errorHandler, storageProvider, question),
        _ => throw new ArgumentException("Invalid question index")
    };
    
}