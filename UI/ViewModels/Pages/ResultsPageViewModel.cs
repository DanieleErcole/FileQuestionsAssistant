using System;
using Core.Questions;
using UI.Services;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public class ResultsPageViewModel(IServiceProvider services) : PageViewModelBase(services) {
    
    private readonly IServiceProvider _services = services;
    public SingleQuestionViewModel QuestionVM { get; private set; }

    public void ToQuestionPage() {
        _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
    }

    public override void OnNavigatedTo(object? param = null) {
        if (param is not AbstractQuestion question) {
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
            return;
        }
        QuestionVM = question.ToViewModel(_services);
    }
    
}