using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using UI.Services;
using UI.Utils;

namespace UI.ViewModels.Questions.Powerpoint;

public abstract class PowerpointQuestionViewModel(AbstractQuestion q, Evaluator evaluator, IErrorHandlerService errorHandler, IStorageService storageService) 
    : SingleQuestionViewModel(q, evaluator, errorHandler, storageService) {
    public override string Icon => "/Assets/pptx.svg";
    protected override FilePickerFileType FileType => FileTypesHelper.Powerpoint;
}