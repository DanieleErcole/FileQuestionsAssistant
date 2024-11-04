using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation; 

public class Evaluator {

    public List<IQuestion<IFile>> Questions { get; } = new();
    public List<IEnumerable<IFile>> Files { get; } = new();

    public IEnumerable<Result> Evaluate<TQuestion>(int index = 0) where TQuestion : IQuestion<IFile> {
        if (Questions.Count < index + 1)
            throw new ArgumentException();
        if(!Files.Any())
            throw new ArgumentException();

        var q = Questions[index];

        if (!q.GetType().IsAssignableTo(typeof(TQuestion)))
            throw new ArgumentException();
        return q.Evaluate(Files[index]);
    }

    public void SetFiles(int index = 0, params IFile[] files) {
        Files[index] = files.AsEnumerable();
    }

}