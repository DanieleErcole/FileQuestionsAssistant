namespace UI.ViewModels.Dialogs;

public class MessageBoxViewModel : DialogContentViewModel {
    
    public string Message { get; }

    public MessageBoxViewModel() : this("Error", "Error, contact support") {}

    public MessageBoxViewModel(string title, string message) : base(title, false, 400, 150) {
        Message = message;
    }
    
}