using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;

namespace Core.Questions;

[method: JsonConstructor]
public abstract class AbstractQuestion(string path, string name, string? desc, MemoryFile ogFile) : IQuestion {
    
    public string Name { get; } = name;
    public string? Desc { get; } = desc;
    public MemoryFile OgFile { get; } = ogFile;

    public string Path { get; set; } = path;
    
    public static IQuestion? DeserializeWithPath(string path, string json, JsonSerializerOptions options) {
        var q = JsonSerializer.Deserialize<AbstractQuestion>(json, options);
        if (q is not null) q.Path = path; // The file could have been moved
        return q;
    }

    public abstract IEnumerable<Result> Evaluate(IEnumerable<IFile> files);
    protected abstract Dictionary<string, object?> ParamsToDict();
    
    private bool Equals(AbstractQuestion? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other.Name == Name && other.Path == Path;
    }
    
    public override bool Equals(object? obj) {
        return Equals(obj as AbstractQuestion);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Path);
    }
    
}