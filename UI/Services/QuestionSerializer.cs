using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.Questions;
using Core.Utils.Errors;
using Serilog;
using ColorConverter = Core.Utils.ColorConverter;

namespace UI.Services;

public class QuestionSerializer(Evaluator evaluator) : ISerializerService {
    private static string TrackedFilePath => Path.Combine(App.AppDataDirectoryPath, TrackedFileName);
    private const string TrackedFileName = "TrackedQuestions.txt";
    
    public static FilePickerFileType FileType { get; } = new("JSON file") {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["application/json"]
    };

    private readonly JsonSerializerOptions _options = new() {
        IncludeFields = true,
        WriteIndented = true,
        Converters = { new ColorConverter() }
    };

    public async Task UpdateTrackingFile() {
        try {
            await using var stream = File.Open(TrackedFilePath, FileMode.Create);
            await using var streamWriter = new StreamWriter(stream);
            foreach (var q in evaluator.Questions)
                await streamWriter.WriteLineAsync(q.Path);
        } catch (Exception e) {
            throw new FileError(TrackedFilePath, e);
        }
    }

    public List<AbstractQuestion?>? LoadTrackedQuestions() {
        if (!Directory.Exists(App.AppDataDirectoryPath))
            Directory.CreateDirectory(App.AppDataDirectoryPath);
        if (!File.Exists(TrackedFilePath)) {
            File.Create(TrackedFilePath).Dispose();
            return null;
        }
        
        try {
            var uniquePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return File.ReadAllLines(TrackedFilePath)
                .Where(p => !string.IsNullOrWhiteSpace(p) && uniquePaths.Add(p))
                .Select(p => {
                    if (string.IsNullOrWhiteSpace(p)) return null;
                    try {
                        using var stream = File.OpenRead(p);
                        using var streamReader = new StreamReader(stream);
                        return AbstractQuestion.DeserializeWithPath(p, streamReader.ReadToEnd(), _options);
                    } catch (Exception e) {
                        Log.Error(e, "Handled error: error while loading tracked questions");
                        return null;
                    }
                }).Where(q => q is not null).ToList();
        } catch (Exception e) {
            Log.Error(e, "Handled error: error while loading tracked questions");
            return null;
        }
    }
    
    public async Task Save(AbstractQuestion question) {
        try {
            await using var stream = File.Open(question.Path, FileMode.Create);
            await using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(question, _options));
        } catch (Exception e) {
            throw new FileError(question.Path, e);
        }
    }
    
    public async Task<AbstractQuestion?> Load(string path) {
        try {
            await using var stream = File.OpenRead(path);
            using var streamReader = new StreamReader(stream);
            return AbstractQuestion.DeserializeWithPath(path, await streamReader.ReadToEndAsync(), _options);
        } catch (JsonException _) {
            throw new InvalidFileFormat(path);
        } catch (Exception e) {
            throw new FileError(path, e);
        }
    }

}