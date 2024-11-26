using System;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using UI.ViewModels;
using UI.Views;
using Control = Avalonia.Controls.Control;

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
    public const int QuestionForm = 1;
    public const int Results = 2;
    
    private Frame _windowFrame;
    private ViewModelBase[] _pages;

    public void Init(Frame windowFrame, IServiceProvider services) {
        _windowFrame = windowFrame;
        _windowFrame.NavigationPageFactory = new AppNavFactory();
        _pages = [
            new QuestionsPageViewModel(services),
            new QuestionDataPageViewModel(),
            new ResultsPageViewModel()
        ];
        NavigateTo(Questions);
    }

    public void NavigateTo(int index) {
        _windowFrame.NavigateFromObject(_pages[index]);
    }
    

}