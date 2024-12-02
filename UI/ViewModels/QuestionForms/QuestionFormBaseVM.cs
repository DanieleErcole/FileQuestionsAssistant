using Core.Questions;

namespace UI.ViewModels.QuestionForms;

public abstract class QuestionFormBaseVM : ViewModelBase {
    public abstract AbstractQuestion CreateQuestion();
}