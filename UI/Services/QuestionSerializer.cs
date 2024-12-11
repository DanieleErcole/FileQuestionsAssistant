using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Utils.Errors;
using ColorConverter = Core.Utils.ColorConverter;

namespace UI.Services;

public class QuestionSerializer {

    private static readonly string TrackedDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FileQuestionAssistant");
    private static string TrackedFilePath => Path.Combine(TrackedDirectoryPath, TrackedFileName);
    private const string TrackedFileName = "TrackedQuestions.txt";
    
    public static FilePickerFileType FileType { get; } = new("JSON file") {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["application/json"]
    };

    private IServiceProvider _services;
    private readonly JsonSerializerOptions? _options = new() {
        IncludeFields = true,
        WriteIndented = true,
        Converters = { new ColorConverter() }
    };

    public void Init(IServiceProvider services) {
        _services = services;
    }

    public async Task UpdateTrackingFile() {
        try {
            await using var stream = File.Open(TrackedFilePath, FileMode.Create);
            await using var streamWriter = new StreamWriter(stream);
            foreach (var q in _services.Get<Evaluator>().Questions)
                await streamWriter.WriteLineAsync(q.Path);
        } catch (Exception e) {
            throw new FileError(TrackedFilePath, e);
        }
    }

    public AbstractQuestion[]? LoadTrackedQuestions() {
        if (!Directory.Exists(TrackedDirectoryPath))
            Directory.CreateDirectory(TrackedDirectoryPath);
        if (!File.Exists(TrackedFilePath)) {
            File.Create(TrackedFilePath);
            return null;
        }
        
        try {
            var paths = File.ReadAllLines(TrackedFilePath);
            return paths.Select(p => {
                if (string.IsNullOrWhiteSpace(p)) return null;
                try {
                    using var stream = File.OpenRead(p);
                    using var streamReader = new StreamReader(stream);
                    return JsonSerializer.Deserialize<AbstractQuestion>(streamReader.ReadToEnd(), _options);
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return null;
                }
            }).Where(q => q is not null).ToArray()!;
        } catch (Exception e) {
            Console.WriteLine(e);
            return null;
        }
    }
    
    public async Task Save(string path, AbstractQuestion question) {
        try {
            await using var stream = File.Open(path, FileMode.Create);
            await using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(question, _options));
        } catch (Exception e) {
            throw new FileError(path, e);
        }
    }
    
    public async Task<AbstractQuestion?> Load(string path) {
        try {
            await using var stream = File.OpenRead(path);
            using var streamReader = new StreamReader(stream);
            return JsonSerializer.Deserialize<AbstractQuestion>(await streamReader.ReadToEndAsync(), _options);
        } catch (JsonException _) {
            throw new InvalidFileFormat(path);
        } catch (Exception e) {
            throw new FileError(path, e);
        }
    }

}