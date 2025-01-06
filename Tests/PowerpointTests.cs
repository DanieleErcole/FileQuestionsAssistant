using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Powerpoint;
using Core.Utils;
using Core.Utils.Errors;
using Tests.Utils;

namespace Tests;

[TestFixture]
public class PowerpointTests {
    
    private static FileStream _powerpointFile;
    private static MemoryFile _ogFile;

    private Evaluator _evaluator;

    [SetUp]
    public void Setup() => _evaluator = new Evaluator();

    [TearDown]
    public void TearDown() => _evaluator.DisposeAllFiles();

    [OneTimeSetUp]
    public void SetupBeforeAll() {
        _powerpointFile = File.Open(TestConstants.TestFilesDirectory + "Presentation1.pptx", FileMode.Open);
        _ogFile = new MemoryFile("OgPresentation.pptx", File.ReadAllBytes(TestConstants.TestFilesDirectory + "OgPresentation.pptx"));
    }

    [OneTimeTearDown]
    public void TearDownAfterAll() => _powerpointFile.Close();

    [Test]
    public void File_NotPresent() => Assert.Throws<FileError>(() => {
        var f = new PowerpointFile(_powerpointFile.Name, _powerpointFile);
        f.Dispose();
        Console.WriteLine($"{f.Shapes}");
    });

    [Test]
    public void File_InvalidFormat() => Assert.Throws<InvalidFileFormat>(() => {
        var f = File.Open(TestConstants.TestFilesDirectory + "InvalidFormat.pptx", FileMode.Open);
        _ = new PowerpointFile(f.Name, f);
    });

    [TestCase(11.8, 0.85, 21.52, 12.11, null, Origin.TopLeftCorner, Origin.TopLeftCorner, false, false, true)]
    [TestCase(-12.48, -3.23, 16.27, 9.15, null, Origin.SlideCenter, Origin.SlideCenter, true, false, true)]
    [TestCase(11.8, null, 21.52, 12.11, null, null, Origin.TopLeftCorner, false, false, true)]
    [TestCase(3.32, 11.52, 5.17, 5.74, 10, null, null, true, false, true)]
    [TestCase(null, null, 5.17, 5.74, 10, null, Origin.TopLeftCorner, true, false, true)]
    [TestCase(11.8, 0.85, 21.52, 12.11, null, Origin.SlideCenter, Origin.SlideCenter, false, true, false)]
    [TestCase(11.8, null, 21.52, 12.11, null, null, Origin.SlideCenter, true, false, false)]
    public void ShapeInsertQuestion_TestCase(double? x, double? y, double? width, double? height, int? rotation, Origin? vO, Origin? hO, bool flipV, bool flipH, bool expectedRes) {
        var q = new ShapeInsertQuestion("", "Name", "Description", _ogFile, x, y, width, height, rotation,
            vO ?? Origin.TopLeftCorner, hO ?? Origin.TopLeftCorner, flipV, flipH);
        _evaluator.AddQuestion(q, new PowerpointFile(_powerpointFile.Name, _powerpointFile));
        
        var res = _evaluator.Evaluate(q).First();
        Assert.That(res.IsSuccessful, Is.EqualTo(expectedRes));
    }

}