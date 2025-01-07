using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using UI.Utils;

namespace UI.ViewModels.Questions;

public abstract class SingleQuestionViewModel(AbstractQuestion q) : ViewModelBase {
    
    public AbstractQuestion Question { get; } = q;
    public string Name => Question.Name;
    public abstract string Description { get; }
    public string Path => Question.Path;
    public abstract string Icon { get; }
    public abstract FilePickerFileType FileType { get; }
    
    public abstract Dictionary<string, object?> GetLocalizedQuestionParams();
    protected abstract List<Dictionary<string, (object?, bool)>> GetLocalizedResultParams(Result res);

    public List<TreeViewNode> ResultParamsAsNodes(Result res) => 
        GetLocalizedResultParams(res)
        .Select(d => { 
            var first = (d.First().Key, d.First().Value.Item1, d.First().Value.Item2);
            var list = d
                .Select(e => (e.Key, e.Value.Item1, e.Value.Item2))
                .Where(e => e.Item1 != first.Item1 && e.Item2 != first.Item2)
                .ToList();
            return new TreeViewNode(first.Item1, first.Item2, first.Item3, list, true);
        }).ToList();

}