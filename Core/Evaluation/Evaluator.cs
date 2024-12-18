using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation; 

public class Evaluator {

    public List<IQuestion> Questions { get; } = [];
    public List<List<IFile>> Files { get; } = [];

    public IEnumerable<Result> Evaluate(int index = 0) {
        if (index >= Questions.Count) 
            throw new ArgumentOutOfRangeException();

        if (Files[index].Count == 0)
            throw new InvalidOperationException();

        return Questions[index].Evaluate(Files[index]);
    }

    public void AddQuestion(IQuestion question, params IFile[] files) {
        Questions.Add(question);
        Files.Add(files.Length != 0 ? files.ToList() : []);
    }

    public void RemoveQuestion(int index) {
        if (index >= Questions.Count)
            throw new ArgumentOutOfRangeException();

        Questions.RemoveAt(index);
        DisposeFiles(index);
        Files.RemoveAt(index);
    }

    public void AddFiles(int index = 0, params IFile[] files) {
        if (index >= Files.Count)
            throw new ArgumentOutOfRangeException();
        Files[index].AddRange(files);
    }

    public void SetFiles(int index = 0, params IFile[] files) {
        if (index >= Files.Count)
            throw new ArgumentOutOfRangeException();

        if (Files[index].Count != 0) 
            DisposeFiles(index);
        Files[index] = files.ToList();
    }

    public void DisposeFiles(int index = 0) {
        if (index >= Files.Count)
            throw new ArgumentOutOfRangeException();
        foreach (var f in Files[index])
            f.Dispose();
        Files[index].Clear();
    }
    
    public void DisposeAllFiles() {
        foreach (var f in Files.SelectMany(f => f))
            f.Dispose();
    }

}