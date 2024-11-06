using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Word;
using Core.Utils;
using Color = System.Drawing.Color;

namespace Tests;

public class UtilsTests {

    [TestCase("FFFFFF", 255, 255, 255)]
    [TestCase("000000", 0, 0, 0)]
    [TestCase("FF0000", 255, 0, 0)]
    [TestCase("00FF00", 0, 255, 0)]
    [TestCase("0000FF", 0, 0, 255)]
    public void HexStringToRgb_TestCase(string hex, int r, int g, int b) {
        var (resR, resG, resB) = hex.HexStringToRgb();
        Assert.That(resR, Is.EqualTo(r));
        Assert.That(resG, Is.EqualTo(g));
        Assert.That(resB, Is.EqualTo(b));
    }

}

public class WordTests {
    
    private const string WordFileDirectory = @"C:\Users\User\Documents\Documenti e lavori\Lavori\C#\FileQuestionsAssistant\Tests\Files\";

    private Evaluator<WordFile> _evaluator;

    [SetUp]
    public void Setup() {
        _evaluator = new Evaluator<WordFile>();
    }

    [TearDown]
    public void TearDown() {
        _evaluator.DisposeAllFiles();
    }

    [TestCase("Document1.docx", "CustomStyle", "Normal", "Consolas", 12, 255, 0, 0, "center", true)]
    [TestCase("Document2.docx", "NotReal", "Normal", "Calibri", 12, 1, 1, 1, "left", false)]
    public void WordCreateStyleQuestion_TestCase(string filePath, string styleName, string baseStyleName, string fontName, int fontSize, int r, int g, int b, string alignment, bool expectedRes) {
        var q = new CreateStyleQuestion(styleName, baseStyleName, fontName, fontSize, Color.FromArgb(r, g, b), alignment);

        _evaluator.Questions.Add(q);
        _evaluator.SetFiles(0, new WordFile[] { new(File.Open(WordFileDirectory + filePath, FileMode.Open)) });

        var res = _evaluator.Evaluate().First();
        Assert.That(res.IsSuccessful, Is.EqualTo(expectedRes));
    }

}