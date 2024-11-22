namespace UI.ViewModels.Dialogs;

public abstract class DialogContentViewModel : ViewModelBase {
    
    public string Title { get; }
    public bool Resizable { get; }
    
    public int InitialWidth { get; }
    public int InitialHeight { get; }

    protected DialogContentViewModel(string title, bool resizable, int width, int height) {
        Title = title;
        Resizable = resizable;
        InitialWidth = width;
        InitialHeight = height;
    }
    
}