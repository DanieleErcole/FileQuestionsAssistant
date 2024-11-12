using ReactiveUI;

namespace UI.ViewModels;

public class QuestionsPageViewModel : ViewModelBase {
    
    private string _greeting = "Ciao";
    public string Greeting {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }

    private int _counter;
    
    public QuestionsPageViewModel() {
        
    }

    public void HandleClick() {
        Greeting = "Ciao" + _counter++;
    }
    
}