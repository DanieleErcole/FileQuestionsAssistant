using UI.Services;

namespace Tests.TestApp.Services;

public class TestErrorHandler : IErrorHandlerService {
    public void ShowError(Exception ex) {
        Console.WriteLine(ex);
    }
}