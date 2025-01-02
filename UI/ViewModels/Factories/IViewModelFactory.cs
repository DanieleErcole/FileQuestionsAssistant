using Core.Questions;
using UI.ViewModels.QuestionForms;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Factories;

public interface IViewModelFactory {
    SingleQuestionViewModel NewQuestionVm(IQuestion question);
    QuestionFormBaseVM NewQuestionFormVm(QuestionTypeIndex index, AbstractQuestion? question = null);
}