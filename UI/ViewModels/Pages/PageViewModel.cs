using Core.Evaluation;
using UI.Services;

namespace UI.ViewModels.Pages;

public abstract class PageViewModel(NavigatorService navService, ErrorHandler errorHandler, QuestionSerializer serializer, Evaluator evaluator) : ViewModelBase {
    
    protected readonly NavigatorService NavigatorService = navService;
    protected readonly ErrorHandler ErrorHandler = errorHandler;
    protected readonly QuestionSerializer Serializer = serializer;
    protected readonly Evaluator Evaluator = evaluator;
    
    public abstract void OnNavigatedTo(object? param = null);
    
}