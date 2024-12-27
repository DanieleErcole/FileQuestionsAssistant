using Core.Evaluation;
using UI.Services;
using UI.ViewModels.Factories;

namespace UI.ViewModels.Pages;

public abstract class PageViewModel(NavigatorService navService, IErrorHandlerService errorHandler, ISerializerService serializer, Evaluator evaluator, 
    IStorageService sService, IViewModelFactory vmFactory) : ViewModelBase {
    
    protected readonly NavigatorService NavigatorService = navService;
    protected readonly IErrorHandlerService ErrorHandler = errorHandler;
    protected readonly ISerializerService Serializer = serializer;
    protected readonly Evaluator Evaluator = evaluator;
    protected readonly IStorageService StorageService = sService;
    protected readonly IViewModelFactory ViewModelFactory = vmFactory;
    
    public abstract void OnNavigatedTo(object? param = null);
    
}