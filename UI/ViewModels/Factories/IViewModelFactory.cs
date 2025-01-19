using Core.Questions;
using UI.ViewModels.QuestionForms;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Factories;

public interface IViewModelFactory {
    QuestionViewModelBase NewQuestionVm(IQuestion question);
    QuestionFormVMBase NewQuestionFormVm(QuestionTypeIndex index, AbstractQuestion? question = null);
}