using System;
using Core.Questions;
using UI.Services;
using UI.ViewModels.QuestionForms;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Factories;

public class QuestionViewModelFactory(IErrorHandlerService errorHandler, IStorageService storageService, QuestionTypeMapper mapper) : IViewModelFactory {

    public QuestionViewModelBase NewQuestionVm(Type type, AbstractQuestion? question = null) =>
        mapper.ViewModel(type, [question])!;

    public QuestionFormVMBase NewQuestionFormVm(Type type, AbstractQuestion? question = null) =>
        mapper.FormViewModel(type, [errorHandler, storageService, question])!;

}