using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using UI.Services;
using UI.ViewModels.Factories;

namespace UI.ViewModels.Pages;

public class QuestionAddPageViewModel(NavigatorService navService, IErrorHandlerService errorHandler, ISerializerService serializer, 
    Evaluator evaluator, IStorageProvider sProvider, IViewModelFactory vmFactory) 
    : QuestionDataPageViewModel(Lang.Lang.NewQuestionPageTitle,  Lang.Lang.CreateBtnText, navService, errorHandler, serializer, evaluator, sProvider, vmFactory) {

    public override void OnNavigatedTo(object? param = null) => SelectedIndex = 0;

    public override async Task ProcessQuestion() {
        try {
            var q = Content?.CreateQuestion();
            if (q is null) return;
            
            var index = Evaluator.Questions.ToList().FindIndex(x => x.Path == q.Path);
            
            await Serializer.Save(q);
            if (index != -1) 
                Evaluator.RemoveQuestion(Evaluator.Questions.ToList().ElementAt(index));
            Evaluator.AddQuestion(q);
            
            await Serializer.UpdateTrackingFile();
            NavigatorService.NavigateTo<ResultsPageViewModel>(q);
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
    }
    
}