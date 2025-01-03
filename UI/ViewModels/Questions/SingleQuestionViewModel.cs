using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Utils.Errors;
using UI.Services;

namespace UI.ViewModels.Questions;

public abstract class SingleQuestionViewModel(AbstractQuestion q, Evaluator evaluator, IErrorHandlerService errorHandler, IStorageService storageService) : ViewModelBase {
    
    public AbstractQuestion Question { get; } = q;
    public string Name => Question.Name;
    public abstract string Description { get; }
    public string Path => Question.Path;
    public abstract string Icon { get; }
    protected abstract FilePickerFileType FileType { get; }
    
    public async Task AddFiles() {
        try {
            var files = await storageService.GetFilesOfTypeAsync(FileType, true);
            if (files.Length == 0)
                return;
            evaluator.AddFiles(Question, files);
        } catch (FileError e) {
            errorHandler.ShowError(e);
        }
    }
    
    public abstract Dictionary<string, object?> GetLocalizedQuestionParams();
    protected abstract List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res);

    public List<FileResultViewModel.Node> ResultParamsAsNodes(Result res) => 
        GetLocalizedResultParams(res)
        .Select(d => { var first = (d.First().Key, d.First().Value.Item1, d.First().Value.Item2);
            var list = d
                .Select(e => (e.Key, e.Value.Item1, e.Value.Item2))
                .Where(e => e.Item1 != first.Item1 && e.Item2 != first.Item2)
                .ToList();
            return new FileResultViewModel.Node(first.Item1, first.Item2, first.Item3, list, true);
        }).ToList();

}