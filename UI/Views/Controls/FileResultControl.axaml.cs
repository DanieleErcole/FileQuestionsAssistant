using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using UI.ViewModels.Questions;
namespace UI.Views.Controls;

public partial class FileResultControl : UserControl {
    
    public FileResultControl() {
        InitializeComponent();
    }
    
    public void OnExpanding(object sender, CancelRoutedEventArgs args) {
        var exp = (sender as Expander)!;
        var vm = (exp.DataContext as FileResultViewModel)!;
        if (!vm.IsExpandable)
            args.Cancel = true;
    }
    
    public void OnSelectionChanged(object sender, SelectionChangedEventArgs args) {
        var tree = (sender as TreeView)!;
        foreach (var item in tree.GetLogicalDescendants().OfType<TreeViewItem>().Where(i => i.IsSelected))
            item.IsSelected = false;
    }
    
}