using Avalonia.Controls;
using Avalonia.Input;
using UI.ViewModels.Pages;

namespace UI.Views.Controls;

public partial class QuestionList : UserControl {
    public QuestionList() {
        InitializeComponent();
    }

    public void OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
        (DataContext as QuestionsPageViewModel)!.OnQuestionSelected(sender, e);
}