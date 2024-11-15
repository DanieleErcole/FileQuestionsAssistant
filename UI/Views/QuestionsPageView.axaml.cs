using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using UI.ViewModels;
using UI.ViewModels.Questions;

namespace UI.Views;

public partial class QuestionsPageView : UserControl {
    
    public QuestionsPageView() {
        InitializeComponent();
    }

    public void OnSelectionChanged(object sender, SelectionChangedEventArgs e) => QuestionsList
        .GetVisualDescendants()
        .OfType<ListBoxItem>()
        .ToList()
        .ForEach(item => {
            var elVm = (item.DataContext as SingleQuestionViewModel)!;
            if (elVm.IsSelected != item.IsSelected)
                elVm.IsSelected = item.IsSelected;
        });

}