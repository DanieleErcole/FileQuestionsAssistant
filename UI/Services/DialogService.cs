using System.Threading.Tasks;
using UI.ViewModels.Dialogs;
using UI.Views;
using UI.Views.Dialogs;

namespace UI.Services;

public class DialogService {

    private readonly MainWindow _mainWindow;

    public DialogService(MainWindow mainWindow) {
        _mainWindow = mainWindow;
    }

    public Task<bool> ShowYesNoDialog() {
        return Task.FromResult(true);
    }
    
    /*public async void ShowMessageDialog(string message, string title = "") {
        //TODO: notifications exists, delete this it's useless now
        if (title == "") title = Lang.Lang.ErrorDialogTitle;
        var dialog = new Dialog {
            DataContext = new DialogViewModel(new MessageBoxViewModel(title, message))
        };
        await dialog.ShowDialog(_mainWindow);
    }*/

}