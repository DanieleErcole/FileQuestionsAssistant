using Core.Evaluation;
using Core.FileHandling;

namespace Core.Questions;

public interface IQuestion {
    IEnumerable<Result> Evaluate(IEnumerable<IFile> files);
}
