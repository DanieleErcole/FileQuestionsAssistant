using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;

namespace Tests;

[TestFixture]
public class EvaluatorTests {
    
    private Evaluator _evaluator;

    private class MyQuestion : IQuestion {
        public string Path { get; set; }

        public IEnumerable<Result> Evaluate(IEnumerable<IFile> files) {
            var d = new Dictionary<string, object?>();
            return files.Select(_ => new Result(d, [], true));
        }
    }

    private class MyFile : IFile {
        public string Name => "";
        public void Dispose() {}
    }
    
    [SetUp]
    public void Setup() => _evaluator = new Evaluator();

    [Test]
    public void Evaluate_ThrowsArgumentOutOfRangeException() {
        Assert.Throws<ArgumentOutOfRangeException>(() => _evaluator.Evaluate(new MyQuestion()));
    }

    [Test]
    public void Evaluate_NoFiles() {
        var q = new MyQuestion();
        _evaluator.AddQuestion(q);
        Assert.Throws<InvalidOperationException>(() => _evaluator.Evaluate(q));
    }

    [Test]
    public void Evaluate_AddQuestion() {
        _evaluator.AddQuestion(new MyQuestion());
        _evaluator.AddQuestion(new MyQuestion());
        Assert.That(_evaluator.Files, Has.Count.EqualTo(_evaluator.Questions.Count));
    }
    
    [Test]
    public void Evaluate_RemoveQuestion() {
        var q = new MyQuestion();
        _evaluator.AddQuestion(q);
        _evaluator.RemoveQuestion(q);
        Assert.That(_evaluator.Files, Has.Count.EqualTo(0));
    }
    
    [Test]
    public void Evaluate_ReplaceQuestion() {
        var q1 = new MyQuestion();
        var q2 = new MyQuestion();
        _evaluator.AddQuestion(q1);
        _evaluator.ReplaceQuestion(q1, q2);
        Assert.That(_evaluator.Questions.First(), Is.EqualTo(q2));
    }
    
    [Test]
    public void Evaluate_AddFiles() {
        var q = new MyQuestion();
        _evaluator.AddQuestion(q);
        _evaluator.AddFiles(q, new MyFile(), new MyFile());
        Assert.That(_evaluator.Files.First(), Has.Count.EqualTo(2));
    }
    
    [Test]
    public void Evaluate_RemoveFile() {
        var q = new MyQuestion();
        var f1 = new MyFile();
        var f2 = new MyFile();
        
        _evaluator.AddQuestion(q);
        _evaluator.AddFiles(q, f1, f2);
        _evaluator.RemoveFile(q, f1);
        
        Assert.That(_evaluator.Files.First().First(), Is.EqualTo(f2));
    }
    
    [Test]
    public void Evaluate_SetFiles() {
        var q = new MyQuestion();
        IFile[] files1 = [new MyFile(), new MyFile()];
        IFile[] files2 = [new MyFile(), new MyFile(), new MyFile()];
        
        _evaluator.AddQuestion(q);
        _evaluator.AddFiles(q, files1);
        _evaluator.SetFiles(q, files2);
        
        Assert.That(_evaluator.Files.First(), Has.Count.EqualTo(files2.Length));
    }

}