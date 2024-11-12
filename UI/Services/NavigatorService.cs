using System.Linq;
using UI.ViewModels;

namespace UI.Services;

public class NavigatorService {

    private MainWindowViewModel _mainWindow;

    private readonly ViewModelBase[] _pages = [
        new QuestionsPageViewModel(),
        new ResultsPageViewModel()
    ];

    public void Init(MainWindowViewModel mw) {
        _mainWindow = mw;
        _mainWindow.CurrentPage = _pages[0];
    }

    public void NavigateTo<T>() where T : ViewModelBase {
        _mainWindow.CurrentPage = _pages.First(p => p.GetType().IsAssignableTo(typeof(T)));
    }

}