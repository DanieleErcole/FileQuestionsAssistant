using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Evaluation;
using UI.Services;

namespace UI.ViewModels.Pages;

public class QuestionAddPageViewModel(IServiceProvider services) : QuestionDataPageViewModel(Lang.Lang.NewQuestionPageTitle,  Lang.Lang.CreateBtnText, services) {

    public override void OnNavigatedTo(object? param = null) => SelectedIndex = 0;

    public override async Task ProcessQuestion() {
        try {
            var q = Content?.CreateQuestion();
            if (q is null) return;

            var ev = _services.Get<Evaluator>();
            var index = ev.Questions.ToList().FindIndex(x => x.Path == q.Path);
            var serializer = _services.Get<QuestionSerializer>();
            
            await serializer.Save(q);
            if (index != -1) 
                ev.RemoveQuestion(q);
            ev.AddQuestion(q);
            
            await serializer.UpdateTrackingFile();
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Results, q);
        } catch (Exception e) {
            _services.Get<ErrorHandler>().ShowError(e);
        }
    }
    
}