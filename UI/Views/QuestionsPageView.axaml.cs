using System.Linq;
using Avalonia.Controls;
using UI.ViewModels.Questions;

namespace UI.Views;

public partial class QuestionsPageView : UserControl {
    
    public QuestionsPageView() {
        InitializeComponent();
    }

    public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)  {
        e.AddedItems
            .OfType<SingleQuestionViewModel>()
            .ToList()
            .ForEach(item => item.IsSelected = true);
        e.RemovedItems
            .OfType<SingleQuestionViewModel>()
            .ToList()
            .ForEach(item => item.IsSelected = false);
    }

}