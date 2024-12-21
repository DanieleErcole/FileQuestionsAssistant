using System;
using FluentAvalonia.UI.Controls;
using UI.ViewModels.Pages;
using UI.Views;
using Control = Avalonia.Controls.Control;
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

    public const int Questions = 0;
    public const int QuestionAddForm = 1;
    public const int QuestionEditForm = 2;
    public const int Results = 3;
    
    private Frame _windowFrame;
    private PageViewModel[] _pages;

    public void Init(Frame windowFrame, IServiceProvider services) {
        _windowFrame = windowFrame;
        _windowFrame.NavigationPageFactory = new AppNavFactory();
        _pages = [
            new QuestionsPageViewModel(services),
            new QuestionAddPageViewModel(services),
            new QuestionEditPageViewModel(services),
            new ResultsPageViewModel(services)
        ];
        NavigateTo(Questions);
    }

    public void NavigateTo(int index, object? param = null) {
        _pages[index].OnNavigatedTo(param);
        _windowFrame.NavigateFromObject(_pages[index]);
    }
    

}