using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace UI.Views.Dialogs;

public partial class MessageBoxView : UserControl {
    
    public MessageBoxView() {
        InitializeComponent();
    }

    public void Ok(object _, RoutedEventArgs e) {
        this.FindAncestorOfType<Window>()?.Close();
    }
    
}