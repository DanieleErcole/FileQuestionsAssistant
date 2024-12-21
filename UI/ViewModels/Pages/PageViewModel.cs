using System;

namespace UI.ViewModels.Pages;

public abstract class PageViewModel(IServiceProvider services) : ViewModelBase {
    protected readonly IServiceProvider _services = services;
    public abstract void OnNavigatedTo(object? param = null);
}