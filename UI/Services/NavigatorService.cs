using System;
using System.Linq;
using UI.ViewModels;

namespace UI.Services;

public class NavigatorService {
    
    public const int Questions = 0;
    public const int Results = 1;
    
    private MainWindowViewModel _mainWindow;

    private ViewModelBase[] _pages;

    public void Init(MainWindowViewModel mw, IServiceProvider services) {
        _mainWindow = mw;
        _pages = new ViewModelBase[] {
            new QuestionsPageViewModel(services),
            new ResultsPageViewModel()
        };
        _mainWindow.CurrentPage = _pages[Questions];
    }

    public void NavigateTo(int index) {
        _mainWindow.CurrentPage = _pages[index];
    }

}