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

}