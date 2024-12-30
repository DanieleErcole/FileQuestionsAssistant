using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Powerpoint;
using Core.Questions.Word;
using UI.Services;
using UI.ViewModels.Factories;

namespace UI.ViewModels.Pages;

public class QuestionEditPageViewModel(NavigatorService navService, IErrorHandlerService errorHandler, ISerializerService serializer, 
    Evaluator evaluator, IStorageService sService, IViewModelFactory vmFactory) 
    : QuestionDataPageViewModel(Lang.Lang.EditQuestionPageTitle,  Lang.Lang.SaveBtnText, navService, errorHandler, serializer, evaluator, sService, vmFactory) {
   
    private static int QuestionToIndex(AbstractQuestion? question = null) {
        return question switch {
            CreateStyleQuestion => QuestionViewModelFactory.CreateStyleQuestionIndex,
            ParagraphApplyStyleQuestion => QuestionViewModelFactory.ParagraphApplyStyleQuestionIndex,
            ImageInsertQuestion => QuestionViewModelFactory.ImageInsertQuestionIndex,
            _ => QuestionViewModelFactory.CreateStyleQuestionIndex
        };
    }

    private AbstractQuestion? _question;

    public override void OnNavigatedTo(object? param = null) {
        var question = param as AbstractQuestion;
        if (question is null)
           NavigatorService.NavigateTo<QuestionsPageViewModel>();
        
        _question = question;
        SelectedIndex = QuestionToIndex(question);
        Content = ViewModelFactory.NewQuestionFormVm(SelectedIndex, question);
    }

    public override async Task ProcessQuestion() {
        if (_question is null) return;
        try {
            var q = Content?.CreateQuestion();
            if (q is null) return;

            await Serializer.Save(q);
            Evaluator.SetFiles(_question);
            
            // I changed the question path, I need to check if I overwrote another question
            if (_question.Path != q.Path && Evaluator.Questions.FirstOrDefault(e => e.Path == q.Path) is {} overwritten)
                Evaluator.RemoveQuestion(overwritten);
            Evaluator.ReplaceQuestion(_question, q);
            
            await Serializer.UpdateTrackingFile();
            NavigatorService.NavigateTo<QuestionsPageViewModel>();
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
    }
    
}