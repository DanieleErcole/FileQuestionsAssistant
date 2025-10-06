using System.Linq;
using System.Threading.Tasks;
using Core.Evaluation;
using UI.Services;
using UI.ViewModels.Factories;

namespace UI.ViewModels.Pages;

public class QuestionAddPageViewModel(NavigatorService navService, IErrorHandlerService errorHandler, ISerializerService serializer, 
    Evaluator evaluator, IStorageService sService, QuestionTypeMapper mapper, IViewModelFactory vmFactory) 
    : QuestionFormPageViewModel(Lang.Lang.NewQuestionPageTitle,  Lang.Lang.CreateBtnText, navService, errorHandler, serializer, evaluator, sService, mapper, vmFactory) {

    public override void OnNavigatedTo(object? param = null) => SelectedIndex = 0;

    public override async Task ProcessQuestion() {
        var q = Content?.CreateQuestion();
        if (q is null) return;
            
        var index = Evaluator.Questions.ToList().FindIndex(x => x.Path == q.Path);
            
        await Serializer.Save(q);
        if (index != -1) 
            Evaluator.RemoveQuestion(Evaluator.Questions.ToList().ElementAt(index));
        Evaluator.AddQuestion(q);
            
        await Serializer.UpdateTrackingFile();
        NavigatorService.NavigateTo<ResultsPageViewModel>(q);
    }
    
}