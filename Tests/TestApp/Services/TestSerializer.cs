using System.Text.Json;
using Core.Evaluation;
using Core.Questions;
using Core.Utils;
using Core.Utils.Errors;
using UI.Services;

namespace Tests.TestApp.Services;

public class TestSerializer(Evaluator evaluator) : ISerializerService {

    private List<string> _jsonQuestions = [];

    private readonly JsonSerializerOptions _options = new() {
        IncludeFields = true,
        WriteIndented = true,
        Converters = { new ColorConverter() }
    };

    public Task UpdateTrackingFile() {
        _jsonQuestions = evaluator.Questions.Select(q => q.Path).ToList();
        return Task.CompletedTask;
    }

    public AbstractQuestion[]? LoadTrackedQuestions() {
        try {
            return _jsonQuestions.Select(p => {
                if (string.IsNullOrWhiteSpace(p)) return null;
                try {
                    using var stream = File.OpenRead(p);
                    using var streamReader = new StreamReader(stream);
                    return AbstractQuestion.DeserializeWithPath(p, streamReader.ReadToEnd(), _options);
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