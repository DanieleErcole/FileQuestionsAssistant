using System.Diagnostics;
using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;
using UI.Views;

namespace UI.Services;

public class DialogService(MainWindow mainWindow) {
    
    public async Task<bool> ShowYesNoDialog(string title, string message) {
        var yesBtn = TaskDialogButton.YesButton;
        yesBtn.Text = Lang.Lang.YesText;
        var noBtn = TaskDialogButton.NoButton;
        noBtn.Text = Lang.Lang.NoText;
        
        var td = new TaskDialog {
            Title = title,
            Content = message,
            Buttons = { yesBtn, noBtn },
            XamlRoot = mainWindow
        };
        return await td.ShowAsync() switch {
            TaskDialogStandardResult.Yes => true,
            TaskDialogStandardResult.No => false,
            _ => throw new UnreachableException()
        };
    }

}