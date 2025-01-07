using System;
using FluentAvalonia.UI.Controls;
using UI.ViewModels.Pages;
using UI.Views;
using Control = Avalonia.Controls.Control;
using QuestionsPageView = UI.Views.Pages.QuestionsPageView;
using ResultsPageView = UI.Views.Pages.ResultsPageView;

namespace UI.Services;

public class NavigatorService {

    private class AppNavFactory : INavigationPageFactory {
        public Control GetPage(Type srcType) {
            throw new NotImplementedException(); // Not used
        }

        public Control GetPageFromObject(object target) {
            return target switch {
                QuestionsPageViewModel => new QuestionsPageView { DataContext = target, },
                QuestionDataPageViewModel=> new QuestionDataPageView { DataContext = target, },
                ResultsPageViewModel => new ResultsPageView { DataContext = target, },
                _ => throw new ArgumentException()
            };
        }
    }

    public static NavigatorService FromServiceProvider(IServiceProvider sp) => new(sp.Get<MainWindow>(), type => 
        type.Name switch { 
            nameof(QuestionsPageViewModel) => sp.Get<QuestionsPageViewModel>(),
            nameof(QuestionAddPageViewModel) => sp.Get<QuestionAddPageViewModel>(),
            nameof(QuestionEditPageViewModel) => sp.Get<QuestionEditPageViewModel>(),
            nameof(ResultsPageViewModel) => sp.Get<ResultsPageViewModel>(),
            _ => throw new ArgumentException("Invalid page type")
        }
    );
    
    private readonly Frame _windowFrame;
    private readonly Func<Type, PageViewModel> _pageFactory;

    public NavigatorService(MainWindow mw, Func<Type, PageViewModel> pageFactory) {
        _windowFrame = mw.MainFrame;
        _windowFrame.NavigationPageFactory = new AppNavFactory();
        _pageFactory = pageFactory;
    }

    public void NavigateTo<TPageVm>(object? param = null) {
        var page = _pageFactory(typeof(TPageVm));
        page.OnNavigatedTo(param);
        _windowFrame.NavigateFromObject(page);
    }

}