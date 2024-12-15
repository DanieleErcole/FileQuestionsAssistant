using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.Questions;
using FluentAvalonia.UI.Data;
using UI.Services;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public partial class ResultsPageViewModel : PageViewModelBase {
    
    private readonly IServiceProvider _services;
    
    [ObservableProperty]
    private SingleQuestionViewModel _questionVM;

    [ObservableProperty]
    private Dictionary<string, object?> _correctParams;
    
    [ObservableProperty]
    private IterableCollectionView? _filesResult;

    private readonly List<Result> _results = [];

    public ResultsPageViewModel(IServiceProvider services) : base(services) {
        _services = services;
    }

    public override void OnNavigatedTo(object? param = null) {
        if (param is not AbstractQuestion question) {
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
            return;
        }
        _results.Clear();
        QuestionVM = question.ToViewModel(_services);
        CorrectParams = QuestionVM.GetLocalizedQuestionParams();
        
        var ev = _services.Get<Evaluator>();
        var files = ev.Files[QuestionVM.Index];
        FilesResult = new IterableCollectionView(files.Select(f => {
            var index = files.IndexOf(f);
            return new FileResultViewModel(QuestionVM, index, f.Name, _results.ElementAtOrDefault(index));
        }), _ => true);
    }
    
    public void ToQuestionPage() {
        _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
    }

    public void RemoveFile(object param) {
        var vm = (param as FileResultViewModel)!;
        _services.Get<Evaluator>().Files[QuestionVM.Index][vm.Index].Dispose();
        _services.Get<Evaluator>().Files[QuestionVM.Index].RemoveAt(vm.Index);
        if (vm.Result is not null)
            _results.RemoveAt(vm.Index);
        FilesResult?.Refresh();
    }

    public async Task AddFiles() {
        await QuestionVM.AddFiles();
        FilesResult?.Refresh();
    }

    public void EvaluateButton() {
        _results.AddRange(_services.Get<Evaluator>().Evaluate(QuestionVM.Index));
        _services.Get<WindowNotificationManager>().ShowNotification(Lang.Lang.EvaluationSuccessTitle, Lang.Lang.EvaluationSuccessDesc, NotificationType.Success);
        FilesResult?.Refresh();
    }
    
}