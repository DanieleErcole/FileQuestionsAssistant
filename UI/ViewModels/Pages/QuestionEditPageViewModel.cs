using System;
using System.Threading.Tasks;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Powerpoint;
using Core.Questions.Word;
using UI.Services;

namespace UI.ViewModels.Pages;

public class QuestionEditPageViewModel(IServiceProvider services) : QuestionDataPageViewModel(Lang.Lang.EditQuestionPageTitle,  Lang.Lang.SaveBtnText, services) {
   
    private static int QuestionToIndex(AbstractQuestion? question = null) {
        return question switch {
            CreateStyleQuestion => 0,
            ParagraphApplyStyleQuestion => 1,
            ImageInsertQuestion => 2,
            _ => 0
        };
    }

    private AbstractQuestion? _question;

    public override void OnNavigatedTo(object? param = null) {
        var question = param as AbstractQuestion;
        if (question is null)
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
        
        _question = question;
        SelectedIndex = QuestionToIndex(question);
        Content = IndexToFormViewModel(SelectedIndex, question);
    }

    public override async Task ProcessQuestion() {
        if (_question is null) return;
        try {
            var q = Content?.CreateQuestion();
            if (q is null) return;

            var ev = _services.Get<Evaluator>();
            var serializer = _services.Get<QuestionSerializer>();

            await serializer.Save(q);
            ev.SetFiles(_question);
            ev.ReplaceQuestion(_question, q);
            
            await serializer.UpdateTrackingFile();
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions, q);
        } catch (Exception e) {
            _services.Get<ErrorHandler>().ShowError(e);
        }
    }
    
}