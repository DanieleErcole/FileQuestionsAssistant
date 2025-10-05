using System;
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

public class QuestionViewModelFactory(IErrorHandlerService errorHandler, IStorageService storageService) : IViewModelFactory {

    public QuestionViewModelBase NewQuestionVm(IQuestion question) => question switch {
        CreateStyleQuestion csq => new CreateStyleQuestionVM(csq),
        ParagraphApplyStyleQuestion par => new ParagraphApplyStyleQuestionVM(par),
        ShapeInsertQuestion pptImage => new ShapeInsertQuestionVM(pptImage),
        _ => throw new ArgumentException("Invalid question type")
    };

    public QuestionFormVMBase NewQuestionFormVm(QuestionTypeIndex index, AbstractQuestion? question = null) => index switch {
        QuestionTypeIndex.CreateStyle 
            => new CreateStyleQuestionFormViewModel(errorHandler, storageService, question as CreateStyleQuestion),
        QuestionTypeIndex.ParagraphApplyStyle 
            => new ParagraphApplyStyleQuestionFormViewModel(errorHandler, storageService, question as ParagraphApplyStyleQuestion),
        QuestionTypeIndex.PptxImageInsert 
            => new ShapeInsertQuestionFormViewModel(errorHandler, storageService, question as ShapeInsertQuestion),
        _ => throw new ArgumentException("Invalid question index")
    };
    
}