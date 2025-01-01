using System;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Powerpoint;
using Core.Questions.Word;
using UI.Services;
using UI.ViewModels.QuestionForms;
using UI.ViewModels.QuestionForms.Powerpoint;
using UI.ViewModels.Questions;
using UI.ViewModels.Questions.Powerpoint;
using UI.ViewModels.Questions.Word;
using CreateStyleQuestionFormViewModel = UI.ViewModels.QuestionForms.Word.CreateStyleQuestionFormViewModel;
using ParagraphApplyStyleQuestionFormViewModel = UI.ViewModels.QuestionForms.Word.ParagraphApplyStyleQuestionFormViewModel;

namespace UI.ViewModels.Factories;

// Note: edit when adding new question types
public enum QuestionTypeIndex {
    CreateStyle = 0,
    ParagraphApplyStyle = 1,
    PptxImageInsert = 2
}

public class QuestionViewModelFactory(Evaluator evaluator, IErrorHandlerService errorHandler, IStorageService storageService) : IViewModelFactory {

    public SingleQuestionViewModel NewQuestionVm(IQuestion question) => question switch {
        CreateStyleQuestion csq => new CreateStyleQuestionVM(csq, evaluator, errorHandler, storageService),
        ParagraphApplyStyleQuestion par => new ParagraphApplyStyleQuestionVM(par, evaluator, errorHandler, storageService),
        ImageInsertQuestion pptImage => new ImageInsertQuestionVM(pptImage, evaluator, errorHandler, storageService),
        _ => throw new ArgumentException("Invalid question type")
    };

    public QuestionFormBaseVM NewQuestionFormVm(QuestionTypeIndex index, AbstractQuestion? question = null) => index switch {
        QuestionTypeIndex.CreateStyle => new CreateStyleQuestionFormViewModel(errorHandler, storageService, question),
        QuestionTypeIndex.ParagraphApplyStyle => new ParagraphApplyStyleQuestionFormViewModel(errorHandler, storageService, question),
        QuestionTypeIndex.PptxImageInsert => new ImageInsertQuestionFormViewModel(errorHandler, storageService, question),
        _ => throw new ArgumentException("Invalid question index")
    };
    
}