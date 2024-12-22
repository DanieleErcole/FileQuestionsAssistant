using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using FluentAvalonia.UI.Data;
using UI.Services;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public partial class ResultsPageViewModel : PageViewModel {
    
    [ObservableProperty]
    private SingleQuestionViewModel _questionVM;
    [ObservableProperty]
    private Dictionary<string, object?> _correctParams;
    [ObservableProperty]
    private IterableCollectionView? _filesResult;
    [ObservableProperty]
    private bool? _checkBoxState = false;
    
    private event EventHandler FileSelected;
    
    private IEnumerable<FileResultViewModel> SelectedFiles => FilesResult!.OfType<FileResultViewModel>().Where(vm => vm.IsSelected);
    private readonly List<Result> _results = [];

    public ResultsPageViewModel(IServiceProvider services) : base(services) {
        FileSelected = (_, _) => {
            CheckBoxState = SelectedFiles.Count() switch {
                0 => false,
                _ => SelectedFiles.Count() == FilesResult!.Count ? true : null
            };
        };
    }

    private void RefreshCheckBoxState() => FileSelected.Invoke(this, EventArgs.Empty);

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
            return new FileResultViewModel(QuestionVM, index, f.Name, _results.ElementAtOrDefault(index), FileSelected);
        }), _ => true);
    }
    
    public void ToQuestionPage() {
        _results.Clear();
        _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
    }

    public void ToggleSelection() {
        if (CheckBoxState is null or false) {
            CheckBoxState = true;
            foreach (var vm in FilesResult!.OfType<FileResultViewModel>())
                vm.IsSelected = true;
        } else {
            CheckBoxState = false;
            foreach (var vm in FilesResult!.OfType<FileResultViewModel>())
                vm.IsSelected = false;
        }
    }

    public void RemoveSelection() {
        var ev = _services.Get<Evaluator>();
        foreach (var item in SelectedFiles) {
            ev.RemoveFile(QuestionVM.Question, item.Index);
            foreach (var next in FilesResult!.OfType<FileResultViewModel>())
                if (next.Index > item.Index) next.Index--;
            
            if (item.Result is not null)
                _results.RemoveAt(item.Index);
        }
        FilesResult?.Refresh();
        RefreshCheckBoxState();
    }

    public async Task AddFiles() {
        await QuestionVM.AddFiles();
        FilesResult?.Refresh();
        RefreshCheckBoxState();
    }

    public void EvaluateButton() {
        try {
            Func<IFile, int, bool>? filterOnlySelected = !SelectedFiles.Any() ? null : 
                (_, index) => SelectedFiles.Any(vm => vm.Index == index);
            
            _results.AddRange( _services.Get<Evaluator>().Evaluate(QuestionVM.Question, filterOnlySelected));
            _services.Get<WindowNotificationManager>().ShowNotification(Lang.Lang.EvaluationSuccessTitle,
                Lang.Lang.EvaluationSuccessDesc, NotificationType.Success);
            FilesResult?.Refresh();
            RefreshCheckBoxState();
        } catch (Exception _) {
            _services.Get<WindowNotificationManager>().ShowNotification(Lang.Lang.NoFilesToEvaluateTitle,
                null, NotificationType.Error);
        }
    }
    
}