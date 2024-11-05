using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation; 

public class WordEvaluator : IEvaluator<IQuestion<WordFile>, WordFile> {

    public List<IQuestion<WordFile>> Questions { get; } = new();
    public List<IEnumerable<WordFile>> Files { get; } = new();

}