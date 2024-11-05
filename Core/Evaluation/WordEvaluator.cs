using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation; 

public class WordEvaluator : IEvaluator<WordFile> {

    public List<IQuestion<WordFile>> Questions { get; } = new();
    public List<IEnumerable<WordFile>> Files { get; } = new();

}