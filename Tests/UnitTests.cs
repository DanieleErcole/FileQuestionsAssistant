using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils;
using Core.Utils.Errors;
using Color = System.Drawing.Color;

namespace Tests;

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

    internal class MyFile : IFile {
        public void Dispose() {}
    }

    internal class MyQuestion : IQuestion<MyFile> {
        public IEnumerable<Result> Evaluate(IEnumerable<MyFile> files) {
            var d = new Dictionary<string, object?>();
            return files.Select(_ => new Result(d, [], true));
        }
    }

    [Test]
    public void Evaluate_ThrowsArgumentOutOfRangeException() {
        var evaluator = new Evaluator<MyFile>();
        Assert.Throws<ArgumentOutOfRangeException>(() => evaluator.Evaluate());
    }

    [Test]
    public void Evaluate_NoFiles() {
        var evaluator = new Evaluator<MyFile>();
        evaluator.AddQuestion(new MyQuestion());
        Assert.Throws<InvalidOperationException>(() => evaluator.Evaluate());
    }

}

[TestFixture]
public class WordTests {
    
#if OS_WINDOWS
    private const string WordFileDirectory = @"C:\Users\User\Documents\Documenti e lavori\Lavori\C#\FileQuestionsAssistant\Tests\Files\";
#elif OS_LINUX
    private const string WordFileDirectory = @"/home/daniele/RiderProjects/FileQuestionsAssistant/Tests/Files/";
#endif
    
    private static FileStream _wordFile;
    private static byte[] _ogFile;

    private Evaluator<WordFile> _evaluator;

    [SetUp]
    public void Setup() => _evaluator = new Evaluator<WordFile>();

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
    public void File_NotPresent() {
        Assert.Throws<FileError>(() => {
            var f = new WordFile(_wordFile.Name, _wordFile);
            f.Dispose();
            Console.WriteLine($"{f.Styles}");
        });
    }

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
        var q = new CreateStyleQuestion("Name", "Description", styleName, Convert.ToBase64String(_ogFile), baseStyleName, fontName, fontSize, rgb, alignment);
        _evaluator.AddQuestion(q, new WordFile(_wordFile.Name, _wordFile));

        var res = _evaluator.Evaluate().First();
        Assert.That(res.IsSuccessful, Is.EqualTo(expectedRes));
    }

}