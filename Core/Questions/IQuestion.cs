using Core.Evaluation;
using Core.FileHandling;

namespace Core.Questions;

public interface IQuestion {
    public QuestionData Data { get; }
    IEnumerable<Result> Evaluate(IEnumerable<IFile> files);
}
