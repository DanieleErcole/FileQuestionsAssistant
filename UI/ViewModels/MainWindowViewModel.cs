using System;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using UI.Services;

namespace UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {

    private readonly IServiceProvider _services;

    private ViewModelBase _currentPage;
    public ViewModelBase CurrentPage { 
        get => _currentPage; 
        set => this.RaiseAndSetIfChanged(ref _currentPage, value); 
    }

    private int _currentIndex = NavigatorService.Questions;

    public int CurrentIndex {
        get => _currentIndex;
        set {
            _currentIndex = value;
            _services.GetRequiredService<NavigatorService>().NavigateTo(_currentIndex);
        }
    }
    
    public MainWindowViewModel(IServiceProvider services) {
        _services = services;
        _services.GetRequiredService<NavigatorService>().Init(this, _services);
    }

}
