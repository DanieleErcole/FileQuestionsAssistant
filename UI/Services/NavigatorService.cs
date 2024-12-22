using System;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Core.Evaluation;
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
    private readonly PageViewModel[] _pages;

    public NavigatorService(ErrorHandler errorHandler, QuestionSerializer serializer, Evaluator evaluator, DialogService dialogService, 
        IStorageProvider storageProvider, WindowNotificationManager notificationManager) {
        _pages = [
            new QuestionsPageViewModel(this, errorHandler, serializer, evaluator,dialogService, storageProvider),
            new QuestionAddPageViewModel(this, errorHandler, serializer, evaluator, storageProvider),
            new QuestionEditPageViewModel(this, errorHandler, serializer, evaluator, storageProvider),
            new ResultsPageViewModel(this, errorHandler, serializer, evaluator, storageProvider, notificationManager)
        ];
    }

    public void Init(Frame windowFrame) {
        _windowFrame = windowFrame;
        _windowFrame.NavigationPageFactory = new AppNavFactory();
        NavigateTo(Questions);
    }

    public void NavigateTo(int index, object? param = null) {
        _pages[index].OnNavigatedTo(param);
        _windowFrame.NavigateFromObject(_pages[index]);
    }
    

}