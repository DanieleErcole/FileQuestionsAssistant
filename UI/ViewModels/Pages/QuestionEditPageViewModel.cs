using System.Linq;
using System.Threading.Tasks;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Powerpoint;
using Core.Questions.Word;
using UI.Services;
using UI.ViewModels.Factories;

namespace UI.ViewModels.Pages;

public class QuestionEditPageViewModel(NavigatorService navService, IErrorHandlerService errorHandler, ISerializerService serializer, 
    Evaluator evaluator, IStorageService sService, QuestionTypeMapper mapper, IViewModelFactory vmFactory) 
    : QuestionFormPageViewModel(Lang.Lang.EditQuestionPageTitle,  Lang.Lang.SaveBtnText, navService, errorHandler, serializer, evaluator, sService, mapper, vmFactory) {

    private AbstractQuestion? _question;

    public override void OnNavigatedTo(object? param = null) {
        var question = param as AbstractQuestion;
        if (question is null)
           NavigatorService.NavigateTo<QuestionsPageViewModel>();
        
        _question = question;
        SelectedIndex = _mapper.IndexFromType(question!.GetType());
        Content = ViewModelFactory.NewQuestionFormVm(question.GetType(), question);
    }

    public override async Task ProcessQuestion() {
        if (_question is null) return;
        var q = Content?.CreateQuestion();
        if (q is null) return;
            
        await Serializer.Save(q);
        Evaluator.SetFiles(_question);
            
        // I changed the question path, I need to check if I overwrote another question
        if (_question.Path != q.Path && Evaluator.Questions.FirstOrDefault(e => e.Path == q.Path) is {} overwritten)
            Evaluator.RemoveQuestion(overwritten);
        Evaluator.ReplaceQuestion(_question, q);
            
        await Serializer.UpdateTrackingFile();
        NavigatorService.NavigateTo<QuestionsPageViewModel>();
    }
    
}