using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions.Word;
using Core.Utils.Errors;
using ReactiveUI;
using UI.Services;
using UI.ViewModels.QuestionForms;

namespace UI.ViewModels;

public class QuestionDataPageViewModel(IServiceProvider services) : PageViewModelBase(services) {

    private string _formType = Lang.Lang.ChooseQuestionTypeText;
    public string FormType {
        get => _formType;
        set => this.RaiseAndSetIfChanged(ref _formType, value);
    }
    
    private ViewModelBase? _content;
    public ViewModelBase? Content {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public override void OnNavigatedTo() {
        FormType = Lang.Lang.ChooseQuestionTypeText;
        Content = null;
    }

    public void ToQuestionPage() {
        _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
    }

    public void SelectQuestionType(int index) {
        Content = index switch {
            0 => new CreateStyleQuestionFormViewModel(),
            _ => throw new UnreachableException()
        };
        FormType = index switch {
            0 => Lang.Lang.CreateStyleQuestionName,
            _ => throw new UnreachableException()
        };
    }
    
    public async Task AddQuestionTest() {
        // Test method do add a new question and return to the Questions page
        var file = await _services.Get<IStorageProvider>().SaveFilePickerAsync(new FilePickerSaveOptions {
            Title = "Save Text File",
            FileTypeChoices = [_services.Get<QuestionSerializer>().FileType],
        });
        if (file is not null) {
            var path = Uri.UnescapeDataString(file.Path.AbsolutePath);
            var q = new CreateStyleQuestion(path, "Nome2", "Desc1", "", "CustomStyle", 
                "Normal", "Consolas", 12, Color.Fuchsia, "center");
            try {
                // Create the question file and add the question to the evaluator list
                var ev = _services.Get<Evaluator>();
                if (await _services.Get<QuestionSerializer>().Create(file, q))
                    // if you overwrote a file first remove from the evaluator list the question with the same path (the overwritten one)
                    ev.RemoveQuestion(ev.Questions.FindIndex(x => x.Path == path));
                ev.AddQuestion(q);
            } catch (FileError e) {
                _services.Get<ErrorHandler>().ShowError(e);
                return;
            }
            _services.Get<NavigatorService>().NavigateTo(NavigatorService.Questions);
        }
    }
    
}