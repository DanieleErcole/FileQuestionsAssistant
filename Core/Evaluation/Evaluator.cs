using System.Collections.ObjectModel;
using Core.FileHandling;
using Core.Questions;

namespace Core.Evaluation;

public class Evaluator {

    public IReadOnlyCollection<IQuestion> Questions => _filesByQuestion.Keys;
    public IReadOnlyCollection<ObservableCollection<IFile>> Files => _filesByQuestion.Values;
    
    private readonly Dictionary<IQuestion, ObservableCollection<IFile>> _filesByQuestion = new();
    
    //TODO: implementare questo con le observablecollection, forse riesco a levarmi dai viewmodel delle domande con gli style trigger

    public IEnumerable<Result> Evaluate(IQuestion question, Func<IFile, int, bool>? fileFilter = null) {
        if (!_filesByQuestion.TryGetValue(question, out var files))
            throw new ArgumentOutOfRangeException();

        if (files.Count == 0)
            throw new InvalidOperationException();

        fileFilter ??= (_, _) => true;
        return question.Evaluate(files.Where(fileFilter));
    }

    public ObservableCollection<IFile> QuestionFiles(IQuestion question) => _filesByQuestion[question];

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
        foreach (var file in files) 
            qFiles.Add(file);
    }

    public void SetFiles(IQuestion question, params IFile[] files) {
        if (!_filesByQuestion.TryGetValue(question, out var qFiles))
            throw new ArgumentOutOfRangeException();

        if (qFiles.Count != 0) 
            DisposeFiles(question);
        _filesByQuestion[question] = new ObservableCollection<IFile>(files);
    }

    public void RemoveFile(IQuestion question, IFile file) {
        if (!_filesByQuestion.TryGetValue(question, out var files))
            throw new ArgumentOutOfRangeException();
        
        if (!files.Contains(file))
            throw new ArgumentOutOfRangeException();
        
        file.Dispose();
        files.Remove(file);
    }

    private void DisposeFiles(IQuestion question) {
        if (!_filesByQuestion.TryGetValue(question, out var files))
            throw new ArgumentOutOfRangeException();
        foreach (var f in files)
            f.Dispose();
        files.Clear();
    }
    
    public void DisposeAllFiles() {
        foreach (var f in _filesByQuestion.Values.SelectMany(f => f))
            f.Dispose();
    }

}