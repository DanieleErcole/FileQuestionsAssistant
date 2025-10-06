using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using Core.Utils.Errors;
using FluentAvalonia.UI.Data;
using UI.Services;
using UI.ViewModels.Factories;
using UI.ViewModels.Questions;

namespace UI.ViewModels.Pages;

public partial class ResultsPageViewModel(NavigatorService navService, IErrorHandlerService errorHandler, ISerializerService serializer, Evaluator evaluator, IStorageService storageService, INotificationService notificationManager, IViewModelFactory vmFactory) : PageViewModel(navService, errorHandler, serializer, evaluator, storageService, vmFactory) {
    
    [ObservableProperty]
    private QuestionViewModelBase? _questionVm;
    [ObservableProperty]
    private Dictionary<string, object?> _correctParams = [];
    [ObservableProperty]
    private IterableCollectionView? _filesResult;
    
    public bool? CheckBoxState => SelectedFiles.Count() switch {
        0 => false,
        _ => SelectedFiles.Count() == FilesResult!.Count ? true : null
    };
    
    private IEnumerable<FileResultViewModel> SelectedFiles => FilesResult?.OfType<FileResultViewModel>().Where(vm => vm.IsSelected) ?? [];
    private readonly Dictionary<IFile, Result> _results = [];

    public void RefreshCheckBoxState() => OnPropertyChanged(nameof(CheckBoxState));

    public override void OnNavigatedTo(object? param = null) {
        if (param is not AbstractQuestion question) {
            NavigatorService.NavigateTo<QuestionsPageViewModel>();
            return;
        }
        QuestionVm = ViewModelFactory.NewQuestionVm(question.GetType(), question);
        CorrectParams = QuestionVm.GetLocalizedQuestionParams();
        
        var files = Evaluator.QuestionFiles(QuestionVm.Question);
        FilesResult = new IterableCollectionView(files.Select(f =>
            new FileResultViewModel(QuestionVm, f, _results.GetValueOrDefault(f))
        ), _ => true);
        RefreshCheckBoxState();
    }
    
    public void ToQuestionPage() {
        _results.Clear();
        QuestionVm = null;
        CorrectParams.Clear();
        NavigatorService.NavigateTo<QuestionsPageViewModel>();
    }
    
    public void ToggleSelection() {
        if (CheckBoxState is null or false)
            foreach (var vm in FilesResult!.OfType<FileResultViewModel>())
                vm.IsSelected = true;
        else
            foreach (var vm in FilesResult!.OfType<FileResultViewModel>())
                vm.IsSelected = false;
        RefreshCheckBoxState();
    }
    
    public void RemoveSelection() {
        foreach (var item in SelectedFiles) {
            Evaluator.RemoveFile(QuestionVm!.Question, item.File);

            if (item.Result is not null)
                _results.Remove(item.File);
        }
        FilesResult?.Refresh();
        RefreshCheckBoxState();
    }
    
    public async Task AddFiles() {
        try {
            var files = await StorageService.GetFilesOfTypeAsync(QuestionVm!.FileType, true);
            if (files.Length == 0)
                return;
            Evaluator.AddFiles(QuestionVm!.Question, files);
        } catch (FileError e) {
            ErrorHandler.ShowError(e);
        }
        FilesResult?.Refresh();
        RefreshCheckBoxState();
    }
    
    public void EvaluateButton() {
        try {
            var anySelected = SelectedFiles.Any();
            var results = Evaluator.Evaluate(QuestionVm!.Question, anySelected ? FilterOnlySelected : null).ToList();
            
            foreach (var result in results) {
                var collection = anySelected ? SelectedFiles : FilesResult!.OfType<FileResultViewModel>();
                var file = collection.ElementAt(results.IndexOf(result)).File;
                if (!_results.TryAdd(file, result))
                    _results[file] = result;
            }
            notificationManager.ShowNotification(Lang.Lang.EvaluationSuccessTitle, Lang.Lang.EvaluationSuccessDesc, NotificationType.Success);
            
            FilesResult?.Refresh();
            RefreshCheckBoxState();
        } catch (Exception _) {
            notificationManager.ShowNotification(Lang.Lang.NoFilesToEvaluateTitle, null, NotificationType.Error);
        }
        
        bool FilterOnlySelected(IFile f, int index) {
            var vm = FilesResult!.OfType<FileResultViewModel>().ElementAtOrDefault(index);
            return vm is not null && vm.File == f && vm.IsSelected;
        }
    }
    
}