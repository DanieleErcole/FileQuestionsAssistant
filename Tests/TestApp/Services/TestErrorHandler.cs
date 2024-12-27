using UI.Services;

namespace Tests.TestApp.Services;

public class TestErrorHandler : IErrorHandlerService {
    
    public List<Exception> Errors { get; } = [];
    
    public void ShowError(Exception ex) {
        Errors.Add(ex);
        Console.WriteLine(ex);
    }
    
}