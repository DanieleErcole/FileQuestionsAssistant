using System.Text.Json;
using Core.Questions;
using Core.Utils;
using UI.Services;

namespace Tests.TestApp.Services;

public class TestSerializer : ISerializerService {
    
    internal readonly List<string> JsonQuestions = [];
    
    private readonly JsonSerializerOptions _options = new() {
        IncludeFields = true,
        WriteIndented = true,
        Converters = { new ColorConverter() }
    };

    public Task UpdateTrackingFile() {
        return Task.CompletedTask;
    }

    public AbstractQuestion[]? LoadTrackedQuestions() =>JsonQuestions
            .Select(json => AbstractQuestion.DeserializeWithPath(JsonQuestions.IndexOf(json).ToString(), json, _options))
            .Where(q => q is not null)
            .ToArray()!;

    public Task Save(AbstractQuestion question) {
        var json = JsonSerializer.Serialize(question, _options);
        if (JsonQuestions.Contains(json))
            JsonQuestions[JsonQuestions.IndexOf(json)] = json;
        else JsonQuestions.Add(json);
        return Task.CompletedTask;
    }

    public Task<AbstractQuestion?> Load(string path) =>
        Task.FromResult(AbstractQuestion.DeserializeWithPath(path, JsonQuestions[int.Parse(path)], _options));
    
}