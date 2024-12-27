using System.Drawing;
using System.Text.Json;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils.Errors;
using Tests.Utils;
using ColorConverter = Core.Utils.ColorConverter;

namespace Tests;

[TestFixture]
public class WordTests {

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
        _wordFile = File.Open(TestConstants.TestFilesDirectory + "Document1.docx", FileMode.Open);
        _ogFile = File.ReadAllBytes(TestConstants.TestFilesDirectory + "OgFile.docx");
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
        var f = File.Open(TestConstants.TestFilesDirectory + "InvalidFormat.docx", FileMode.Open);
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
        var res = _evaluator.Evaluate(q).First();
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
        var res = _evaluator.Evaluate(q).First();
        
        ResHelper.LogResult(res);
        Assert.That(res.IsSuccessful, Is.EqualTo(expectedRes));
    }

}