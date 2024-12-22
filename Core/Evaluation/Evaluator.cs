using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation;

public class Evaluator {

    public IReadOnlyCollection<IQuestion> Questions => _filesByQuestion.Keys;
    public IReadOnlyCollection<IReadOnlyCollection<IFile>> Files => _filesByQuestion.Values;

    private readonly Dictionary<IQuestion, List<IFile>> _filesByQuestion = new();

    public IEnumerable<Result> Evaluate(IQuestion question, Func<IFile, int, bool>? fileFilter = null) {
        if (!_filesByQuestion.TryGetValue(question, out var files))
            throw new ArgumentOutOfRangeException();

        if (files.Count == 0)
            throw new InvalidOperationException();

        fileFilter ??= (_, _) => true;
        return question.Evaluate(files.Where(fileFilter));
    }

    public List<IFile> QuestionFiles(IQuestion question) => _filesByQuestion[question];

    public void AddQuestion(IQuestion question, params IFile[] files) => _filesByQuestion.Add(question, [..files]);

    public void RemoveQuestion(IQuestion question) {
        if (!_filesByQuestion.ContainsKey(question))
            throw new ArgumentOutOfRangeException();
        
        DisposeFiles(question);
        _filesByQuestion.Remove(question);
    }

    public void ReplaceQuestion(IQuestion oldQuestion, IQuestion newQuestion) {
        var files = _filesByQuestion[oldQuestion];
        _filesByQuestion.Remove(oldQuestion);
        AddQuestion(newQuestion, files.ToArray());
    }

    public void AddFiles(IQuestion question, params IFile[] files) {
        if (!_filesByQuestion.TryGetValue(question, out var qFiles))
            throw new ArgumentOutOfRangeException();
        qFiles.AddRange(files);
    }

    public void SetFiles(IQuestion question, params IFile[] files) {
        if (!_filesByQuestion.TryGetValue(question, out var qFiles))
            throw new ArgumentOutOfRangeException();

        if (qFiles.Count != 0) 
            DisposeFiles(question);
        _filesByQuestion[question] = files.ToList();
    }

    public void RemoveFile(IQuestion question, int index) {
        if (!_filesByQuestion.TryGetValue(question, out var files))
            throw new ArgumentOutOfRangeException();
        
        if (files.Count <= index)
            throw new ArgumentOutOfRangeException();
        
        files[index].Dispose();
        files.RemoveAt(index);
    }

    public void DisposeFiles(IQuestion question) {
        if (!_filesByQuestion.TryGetValue(question, out var files))
            throw new ArgumentOutOfRangeException();
        foreach (var f in files)
            f.Dispose();
        _filesByQuestion.Clear();
    }
    
    public void DisposeAllFiles() {
        foreach (var f in _filesByQuestion.Values.SelectMany(f => f))
            f.Dispose();
    }

}