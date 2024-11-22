using Avalonia.Controls;
using UI.ViewModels.Dialogs;

namespace UI.Views.Dialogs;

public partial class Dialog : Window {
    private DialogViewModel ViewModel => (DialogViewModel) DataContext!;
    
    public Dialog() {
        InitializeComponent();
        DataContextChanged += (_, _) => ViewModel.DialogContent.CloseRequested += (_, param) => Close(param); 
    }
}