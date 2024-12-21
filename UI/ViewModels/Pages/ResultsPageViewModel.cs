using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using DynamicData;
using FluentAvalonia.UI.Data;
using UI.Services;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public partial class ResultsPageViewModel(IServiceProvider services) : PageViewModel(services) {
    
    [ObservableProperty]
    private SingleQuestionViewModel _questionVM;
    [ObservableProperty]
    private Dictionary<string, object?> _correctParams;
    [ObservableProperty]
    private IterableCollectionView? _filesResult;
    [ObservableProperty]
    private bool? _checkBoxState;
    
    private IEnumerable<FileResultViewModel> SelectedFiles => FilesResult!.OfType<FileResultViewModel>().Where(vm => vm.IsSelected);
    private readonly List<Result> _results = [];

    public override void OnNavigatedTo(object? param = null) {
        if (param is not AbstractQuestion question) {
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
            return;
        }
        QuestionVM = question.ToViewModel(_services);
        CorrectParams = QuestionVM.GetLocalizedQuestionParams();
        
        var ev = _services.Get<Evaluator>();
        var files = ev.QuestionFiles(QuestionVM.Question);
        FilesResult = new IterableCollectionView(files.Select(f => {
            var index = files.IndexOf(f);
            return new FileResultViewModel(QuestionVM, index, f.Name, _results.ElementAtOrDefault(index));
        }), _ => true);
        
        // TODO: non va, molto prob la classe deve essere un ReactiveObject
        FilesResult.OfType<FileResultViewModel>()
            .AsObservableChangeSet()
            .WhenValueChanged(vm => vm.IsSelected)
            .Subscribe(_ => {
                CheckBoxState = SelectedFiles.Count() switch {
                    0 => false,
                    _ => SelectedFiles.Count() == FilesResult.Count ? true : null
                };
            });
    }
    
    public void ToQuestionPage() {
        _results.Clear();
        _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
    }

    public void RemoveSelection() {
        var ev = _services.Get<Evaluator>();
        foreach (var item in SelectedFiles) {
            var files = ev.QuestionFiles(QuestionVM.Question);
            
            files.ElementAt(item.Index).Dispose();
            files.RemoveAt(item.Index);
            
            if (item.Result is not null)
                _results.RemoveAt(item.Index);
        }
    }

    public async Task AddFiles() {
        await QuestionVM.AddFiles();
        FilesResult?.Refresh();
    }

    public void EvaluateButton() {
        try {
            Func<IFile, int, bool>? filterOnlySelected = !SelectedFiles.Any() ? null : 
                (_, index) => SelectedFiles.Any(vm => vm.Index == index);
            
            _results.AddRange( _services.Get<Evaluator>().Evaluate(QuestionVM.Question, filterOnlySelected));
            _services.Get<WindowNotificationManager>().ShowNotification(Lang.Lang.EvaluationSuccessTitle,
                Lang.Lang.EvaluationSuccessDesc, NotificationType.Success);
            FilesResult?.Refresh();
        } catch (Exception _) {
            _services.Get<WindowNotificationManager>().ShowNotification(Lang.Lang.NoFilesToEvaluateTitle,
                null, NotificationType.Error);
        }
    }
    
}