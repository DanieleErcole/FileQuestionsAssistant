using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation; 

public class Evaluator<TFile> where TFile : IFile {

    public List<IQuestion<TFile>> Questions { get; } = new();
    public List<IEnumerable<TFile>> Files { get; } = new();

    public IEnumerable<Result> Evaluate(int index = 0) {
        if (Questions.Count < index + 1) 
            throw new ArgumentOutOfRangeException();
        if (Questions.Count != Files.Count || !Files.Any())
            throw new InvalidOperationException();

        var q = Questions[index];

        if (!q.GetType().IsAssignableTo(typeof(IQuestion<TFile>)))
            throw new InvalidOperationException();
        return q.Evaluate(Files[index]);
    }

    public void SetFiles(int index = 0, params TFile[] files) {
        if (index > Files.Count)
            throw new ArgumentOutOfRangeException();

        if (index == Files.Count) {
            Files.Add(files.AsEnumerable());
            return;
        }

        Files[index] = files.AsEnumerable();
    }

    public void DisposeAllFiles() {
        foreach (var f in Files.SelectMany(files => files))
            f.Dispose();
    }

}