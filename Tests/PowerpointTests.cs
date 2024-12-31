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
        Console.WriteLine($"{f.Pictures}");
    });

    [Test]
    public void File_InvalidFormat() => Assert.Throws<InvalidFileFormat>(() => {
        var f = File.Open(TestConstants.TestFilesDirectory + "InvalidFormat.pptx", FileMode.Open);
        _ = new PowerpointFile(f.Name, f);
    });

    [TestCase(11.8, 0.85, 21.52, 12.11, Origin.TopLeftCorner, Origin.TopLeftCorner, true)]
    [TestCase(11.8, null, 21.52, 12.11, null, Origin.TopLeftCorner, true)]
    [TestCase(11.8, 0.85, 21.52, 12.11, Origin.SlideCenter, Origin.SlideCenter, false)]
    [TestCase(11.8, null, 21.52, 12.11, null, Origin.SlideCenter, false)]
    public void ImageInsertQuestion_TestCase(double? x, double? y, double? width, double? height, Origin? vO, Origin? hO, bool expectedRes) {
        var q = new ImageInsertQuestion("", "Name", "Description", _ogFile, x, y, width, height, vO, hO);
        _evaluator.AddQuestion(q, new PowerpointFile(_powerpointFile.Name, _powerpointFile));
        
        var res = _evaluator.Evaluate(q).First();
        ResHelper.LogResult(res);
        Assert.That(res.IsSuccessful, Is.EqualTo(expectedRes));
    }

}