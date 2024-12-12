using Core.Evaluation;

namespace UI.ViewModels.Questions;

public class FileResultViewModel : ViewModelBase {
    
    public int Index { get; }
    public string Filename { get; }
    public Result? Result { get; }
    
    public bool IsSuccess => Result is { IsSuccessful: true };
    public bool IsFailed => Result is { IsSuccessful: false };

    public string Icon => Result switch {
        {} res => res.IsSuccessful ? "Checkmark" : "Dismiss",
        _ => "Document"
    };

    public FileResultViewModel(int index, string filename, Result? result) {
        Index = index;
        Filename = filename;
        Result = result;
    }
    
}