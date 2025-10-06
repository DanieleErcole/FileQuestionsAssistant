using System;
using Core.Questions;
using UI.Services;
using UI.ViewModels.QuestionForms;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Factories;

public class QuestionViewModelFactory(IErrorHandlerService errorHandler, IStorageService storageService, QuestionTypeMapper mapper) : IViewModelFactory {

    public QuestionViewModelBase NewQuestionVm(Type type, AbstractQuestion? question = null) =>
        (mapper.GetViewModelConstructor(type).Invoke([question]) as QuestionViewModelBase)!;

    public QuestionFormVMBase NewQuestionFormVm(Type type, AbstractQuestion? question = null) =>
        (mapper.GetFormConstructor(type).Invoke([errorHandler, storageService, question]) as QuestionFormVMBase)!;
    
}