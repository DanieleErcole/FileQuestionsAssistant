using System.Threading.Tasks;

namespace UI.Services;

public interface IDialogService {
    Task<bool> ShowYesNoDialog(string title, string message);
}