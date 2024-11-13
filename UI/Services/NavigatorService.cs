using System;
using System.Linq;
using UI.ViewModels;

namespace UI.Services;

public class NavigatorService {
    
    private MainWindowViewModel _mainWindow;

    private ViewModelBase[] _pages;

    public void Init(MainWindowViewModel mw, IServiceProvider services) {
        _mainWindow = mw;
        _pages = new ViewModelBase[] {
            new QuestionsPageViewModel(services),
            new ResultsPageViewModel()
        };
        _mainWindow.CurrentPage = _pages[0];
    }

    public void NavigateTo<T>() where T : ViewModelBase {
        _mainWindow.CurrentPage = _pages.First(p => p.GetType().IsAssignableTo(typeof(T)));
    }

}