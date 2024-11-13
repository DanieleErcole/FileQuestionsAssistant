using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation; 

public class Evaluator<TFile> where TFile : IFile {

    public List<IQuestion<TFile>> Questions { get; } = [];
    public List<IEnumerable<TFile>> Files { get; } = [];

    public IEnumerable<Result> Evaluate(int index = 0) {
        if (index >= Questions.Count) 
            throw new ArgumentOutOfRangeException();

        if (!Files[index].Any())
            throw new InvalidOperationException();

        return Questions[index].Evaluate(Files[index]);
    }

    public void AddQuestion(IQuestion<TFile> question, params TFile[] files) {
        Questions.Add(question);
        Files.Add(files.Length != 0 ? files : new List<TFile>());
    }

    public void RemoveQuestion(int index) {
        if (index > Questions.Count)
            throw new ArgumentOutOfRangeException();

        Questions.RemoveAt(index);
        Files.RemoveAt(index);
    }

    public void SetFiles(int index = 0, params TFile[] files) {
        if (index >= Files.Count)
            throw new ArgumentOutOfRangeException();

        Files[index] = files.AsEnumerable();
    }

    public void DisposeAllFiles() {
        foreach (var f in Files.SelectMany(files => files))
            f.Dispose();
    }

}