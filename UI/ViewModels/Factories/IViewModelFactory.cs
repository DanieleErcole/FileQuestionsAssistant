using System;
using Core.Questions;
using UI.ViewModels.QuestionForms;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Factories;

public interface IViewModelFactory {
    QuestionViewModelBase NewQuestionVm(Type type, AbstractQuestion? question = null);
    QuestionFormVMBase NewQuestionFormVm(Type type, AbstractQuestion? question = null);
}