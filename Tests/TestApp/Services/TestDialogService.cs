using UI.Services;

namespace Tests.TestApp.Services;

public class TestDialogService : IDialogService {
    public Task<bool> ShowYesNoDialog(string title, string message) => Task.FromResult(true);
}