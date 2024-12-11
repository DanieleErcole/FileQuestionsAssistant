using System;
using System.Collections.Generic;
using Core.Questions;
using ReactiveUI;
using UI.Services;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public class ResultsPageViewModel : PageViewModelBase {
    
    private readonly IServiceProvider _services;
    
    private SingleQuestionViewModel _questionVm;
    public SingleQuestionViewModel QuestionVM {
        get => _questionVm;
        private set => this.RaiseAndSetIfChanged(ref _questionVm, value);
    }

    private Dictionary<string, object?> _correctParams;
    public Dictionary<string, object?> CorrectParams {
        get => _correctParams;
        private set => this.RaiseAndSetIfChanged(ref _correctParams, value);
    }

    public ResultsPageViewModel(IServiceProvider services) : base(services) {
        _services = services;
    }

    public override void OnNavigatedTo(object? param = null) {
        if (param is not AbstractQuestion question) {
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
            return;
        }
        QuestionVM = question.ToViewModel(_services);
        CorrectParams = QuestionVM.GetLocalizedQuestionParams();
    }
    
    public void ToQuestionPage() => _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
    
}