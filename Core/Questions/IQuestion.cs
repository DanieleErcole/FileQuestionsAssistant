using Core.Evaluation;
using Core.FileHandling;

namespace Core.Questions;

public interface IQuestion {
    public string Path { get; }
    IEnumerable<Result> Evaluate(IEnumerable<IFile> files);
}
