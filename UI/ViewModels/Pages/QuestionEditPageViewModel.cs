using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Questions.Powerpoint;
using Core.Questions.Word;
using UI.Services;

namespace UI.ViewModels.Pages;

public class QuestionEditPageViewModel(NavigatorService navService, ErrorHandler errorHandler, QuestionSerializer serializer, Evaluator evaluator, IStorageProvider sProvider) 
    : QuestionDataPageViewModel(Lang.Lang.EditQuestionPageTitle,  Lang.Lang.SaveBtnText, navService, errorHandler, serializer, evaluator, sProvider) {
   
    private static int QuestionToIndex(AbstractQuestion? question = null) {
        return question switch {
            CreateStyleQuestion => 0,
            ParagraphApplyStyleQuestion => 1,
            ImageInsertQuestion => 2,
            _ => 0
        };
    }

    private AbstractQuestion? _question;

    public override void OnNavigatedTo(object? param = null) {
        var question = param as AbstractQuestion;
        if (question is null)
           NavigatorService.NavigateTo(NavigatorService.Questions);
        
        _question = question;
        SelectedIndex = QuestionToIndex(question);
        Content = IndexToFormViewModel(SelectedIndex, question);
    }

    public override async Task ProcessQuestion() {
        if (_question is null) return;
        try {
            var q = Content?.CreateQuestion();
            if (q is null) return;

            await Serializer.Save(q);
            Evaluator.SetFiles(_question);
            Evaluator.ReplaceQuestion(_question, q);
            
            await Serializer.UpdateTrackingFile();
            NavigatorService.NavigateTo(NavigatorService.Questions, q);
        } catch (Exception e) {
            ErrorHandler.ShowError(e);
        }
    }
    
}