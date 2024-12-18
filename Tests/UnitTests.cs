using System.Text.Json;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils;
using Core.Utils.Errors;
using Color = System.Drawing.Color;

namespace Tests;

public class ResHelper {
    public static void LogResult(Result res) {
        foreach (var p in res.EachParamsWithRes()) {
            foreach (var (key, value) in p)
                Console.WriteLine($"{key}: {value}");
            Console.WriteLine("---------------");
        }
    }
}

[TestFixture]
public class UtilsTests {
         
    [TestCase("FFFFFF", 255, 255, 255)]
    [TestCase("000000", 0, 0, 0)]
    [TestCase("FF0000", 255, 0, 0)]
    [TestCase("00FF00", 0, 255, 0)]
    [TestCase("0000FF", 0, 0, 255)]
    public void HexStringToRgb_TestCase(string hex, int r, int g, int b) => Assert.That((r, g, b), Is.EqualTo(hex.HexStringToRgb()));

}

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
        Assert.Throws<ArgumentOutOfRangeException>(() => _evaluator.Evaluate());
    }

    [Test]
    public void Evaluate_NoFiles() {
        _evaluator.AddQuestion(new MyQuestion());
        Assert.Throws<InvalidOperationException>(() => _evaluator.Evaluate());
    }

    [Test]
    public void Evaluate_AddQuestion() {
        _evaluator.AddQuestion(new MyQuestion());
        _evaluator.AddQuestion(new MyQuestion());
        Assert.That(_evaluator.Files, Has.Count.EqualTo(_evaluator.Questions.Count));
    }
    
    [Test]
    public void Evaluate_RemoveQuestion() {
        _evaluator.AddQuestion(new MyQuestion());
        _evaluator.RemoveQuestion(0);
        Assert.That(_evaluator.Files, Has.Count.EqualTo(0));
    }

}

[TestFixture]
public class WordTests {
    
#if OS_WINDOWS
    private const string WordFileDirectory = @"C:\Users\User\Documents\Documenti e lavori\Lavori\C#\FileQuestionsAssistant\Tests\Files\";
#elif OS_LINUX
    private const string WordFileDirectory = @"/home/daniele/RiderProjects/FileQuestionsAssistant/Tests/Files/";
#endif

    private static readonly JsonSerializerOptions Options = new() { Converters = { new ColorConverter() } };
    private static FileStream _wordFile;
    private static byte[] _ogFile;

    private Evaluator _evaluator;

    [SetUp]
    public void Setup() => _evaluator = new Evaluator();

    [TearDown]
    public void TearDown() => _evaluator.DisposeAllFiles();

    [OneTimeSetUp]
    public void SetupBeforeAll() {
        _wordFile = File.Open(WordFileDirectory + "Document1.docx", FileMode.Open);
        _ogFile = File.ReadAllBytes(WordFileDirectory + "OgFile.docx");
    }

    [OneTimeTearDown]
    public void TearDownAfterAll() => _wordFile.Close();

    [Test]
    public void File_NotPresent() => Assert.Throws<FileError>(() => {
        var f = new WordFile(_wordFile.Name, _wordFile);
        f.Dispose();
        Console.WriteLine($"{f.Styles}");
    });

    [Test]
    public void File_InvalidFormat() => Assert.Throws<InvalidFileFormat>(() => {
        var f = File.Open(WordFileDirectory + "InvalidFormat.docx", FileMode.Open);
        _ = new WordFile(f.Name, f);
    });

    [TestCase("CustomStyle", "Normal", "Consolas", 12, 255, 0, 0, "center", true)]
    [TestCase("NotReal", "Normal", "Calibri", 12, 1, 1, 1, "left", false)]
    [TestCase("CustomStyle", "Normal", "Consolas", null, null, 0, 0, null, true)]
    [TestCase("WrongName", "Heading 1", "Calibri", null, 255, 0, 0, null, false)]
    public void WordCreateStyleQuestion_TestCase(string styleName, string? baseStyleName, string? fontName, int? fontSize, int? r, int? g, int? b, string? alignment, bool expectedRes) {
        Color? rgb = r is null || g is null || b is null ? null : Color.FromArgb((int) r, (int) g, (int) b);
        var q = new CreateStyleQuestion("", "Name", "Description", _ogFile, styleName, baseStyleName, fontName, fontSize, rgb, alignment);
        
        _evaluator.AddQuestion(q, new WordFile(_wordFile.Name, _wordFile));
        var res = _evaluator.Evaluate().First();
        Assert.That(res.IsSuccessful, Is.EqualTo(expectedRes));
    }

    [Test]
    public void WordCreateStyleQuestion_polymorphic_serialization() {
        AbstractQuestion q = new CreateStyleQuestion("", "Name", "Description", _ogFile, "styleName", 
            "baseStyleName", "fontName", 12, Color.Blue, "alignment");
        var json = JsonSerializer.Serialize(q, Options);
        var deserialized = JsonSerializer.Deserialize<AbstractQuestion>(json, Options);
        Assert.That(deserialized is CreateStyleQuestion, Is.EqualTo(true));
    }

    [TestCase("CustomStyle", true)]
    [TestCase("Wow", true)]
    [TestCase("Heading 1", false)]
    public void ParagraphApplyStyleQuestion_TestCase(string styleName, bool expectedRes) {
        var q = new ParagraphApplyStyleQuestion("", "Name", "Description", _ogFile, styleName);
        _evaluator.AddQuestion(q, new WordFile(_wordFile.Name, _wordFile));
        var res = _evaluator.Evaluate().First();
        
        ResHelper.LogResult(res);
        Assert.That(res.IsSuccessful, Is.EqualTo(expectedRes));
    }

}

[TestFixture]
public class PowerpointTests {
    
#if OS_WINDOWS
    private const string PowerpointFileDirectory = @"C:\Users\User\Documents\Documenti e lavori\Lavori\C#\FileQuestionsAssistant\Tests\Files\";
#elif OS_LINUX
    private const string PowerpointFileDirectory = @"/home/daniele/RiderProjects/FileQuestionsAssistant/Tests/Files/";
#endif

    private static readonly JsonSerializerOptions Options = new() { Converters = { new ColorConverter() } };
    private static FileStream _powerpointFile;
    private static byte[] _ogFile;

    private Evaluator _evaluator;

    [SetUp]
    public void Setup() => _evaluator = new Evaluator();

    [TearDown]
    public void TearDown() => _evaluator.DisposeAllFiles();

    [OneTimeSetUp]
    public void SetupBeforeAll() {
        _powerpointFile = File.Open(PowerpointFileDirectory + "Presentation1.pptx", FileMode.Open);
        _ogFile = File.ReadAllBytes(PowerpointFileDirectory + "OgPresentation.pptx");
    }

    [OneTimeTearDown]
    public void TearDownAfterAll() => _powerpointFile.Close();

    [Test]
    public void File_NotPresent() => Assert.Throws<FileError>(() => {
        var f = new PowerpointFile(_powerpointFile.Name, _powerpointFile);
        f.Dispose();
        Console.WriteLine($"{f.Pictures}");
    });

    [Test]
    public void File_InvalidFormat() => Assert.Throws<InvalidFileFormat>(() => {
        var f = File.Open(PowerpointFileDirectory + "InvalidFormat.pptx", FileMode.Open);
        _ = new PowerpointFile(f.Name, f);
    });

}