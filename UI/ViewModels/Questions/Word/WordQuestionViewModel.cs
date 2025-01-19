using Avalonia.Platform.Storage;
using Core.Questions;
using UI.Utils;

namespace UI.ViewModels.Questions.Word;

public abstract class WordQuestionViewModel(AbstractQuestion q) : QuestionViewModelBase(q) {
    public override string Icon => "/Assets/docx.svg";
    public override FilePickerFileType FileType => FileTypesHelper.Word;
}