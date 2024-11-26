using System;
using System.Linq;
using Avalonia.Controls;
using UI.ViewModels;
using UI.ViewModels.Questions;

namespace UI.Views;

public partial class QuestionsPageView : UserControl {
    
    public QuestionsPageView() {
        InitializeComponent();
    }

    public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)  {
        (DataContext as QuestionsPageViewModel)!.OnSelectedQuestion(e);
    }
}