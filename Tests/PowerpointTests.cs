using System.Text.Json;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using Core.Utils.Errors;

namespace Tests;

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