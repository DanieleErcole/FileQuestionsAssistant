using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation; 

public interface IEvaluator<TFile> where TFile : IFile {

    public List<IQuestion<TFile>> Questions { get; }
    public List<IEnumerable<TFile>> Files { get; }

    IEnumerable<Result> Evaluate(int index = 0) {
        if (Questions.Count < index + 1)
            throw new ArgumentOutOfRangeException();
        if (!Files.Any())
            throw new ArgumentException();

        var q = Questions[index];

        if (!q.GetType().IsAssignableTo(typeof(IQuestion<TFile>)))
            throw new ArgumentException();
        return q.Evaluate(Files[index]);
    }

    void SetFiles(int index = 0, params TFile[] files) {
        Files[index] = files.AsEnumerable();
    }

}