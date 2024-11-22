

namespace UI.ViewModels.Dialogs;

public class DialogViewModel : ViewModelBase {
    public DialogContentViewModel DialogContent { get; }

    public DialogViewModel(DialogContentViewModel dialogContent) {
        DialogContent = dialogContent;
    }
}