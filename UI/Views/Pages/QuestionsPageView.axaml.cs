using Avalonia.Controls;
using UI.ViewModels.Pages;

namespace UI.Views;

public partial class QuestionsPageView : UserControl {
    
    public QuestionsPageView() {
        InitializeComponent();
    }

    public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)  {
        (DataContext as QuestionsPageViewModel)!.OnSelectedQuestion(e);
    }
}