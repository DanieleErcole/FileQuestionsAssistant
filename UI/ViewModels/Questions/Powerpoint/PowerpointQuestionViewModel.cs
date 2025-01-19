using Avalonia.Platform.Storage;
using Core.Questions;
using UI.Utils;

namespace UI.ViewModels.Questions.Powerpoint;

public abstract class PowerpointQuestionViewModel(AbstractQuestion q) : QuestionViewModelBase(q) {
    public override string Icon => "/Assets/pptx.svg";
    public override FilePickerFileType FileType => FileTypesHelper.Powerpoint;
}