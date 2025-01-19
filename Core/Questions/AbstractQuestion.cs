using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Powerpoint;
using Core.Questions.Word;
using Core.Utils;

namespace Core.Questions;

// Note: edit when adding new question types
[JsonDerivedType(typeof(CreateStyleQuestion), typeDiscriminator: 0)]
[JsonDerivedType(typeof(ParagraphApplyStyleQuestion), typeDiscriminator: 1)]
[JsonDerivedType(typeof(ShapeInsertQuestion), typeDiscriminator: 2)]
public abstract class AbstractQuestion : IQuestion {
    
    public string Name { get; }
    public string? Desc { get; }
    public MemoryFile OgFile { get; }

    [JsonInclude]
    public readonly Dictionary<string, object?> Params = new();
    
    [JsonIgnore]
    public string Path { get; set; } // The path will be initialized when creating the question from code and after deserializing it, it will be always initialized

    public static IQuestion? DeserializeWithPath(string path, string json, JsonSerializerOptions options) {
        var q = JsonSerializer.Deserialize<AbstractQuestion>(json, options);
        if (q is not null) q.Path = path;
        return q;
    }
    
    [JsonConstructor]
    public AbstractQuestion(string name, string desc, MemoryFile ogFile, Dictionary<string, object?> Params) {
        Name = name;
        Desc = desc;
        OgFile = ogFile;
        this.Params = Params;
        // The virtual method changes the Params initialised above,
        // ìgnore the warning
        DeserializeParams();
    }

    protected AbstractQuestion(string path, string name, string? desc, MemoryFile ogFile) {
        Name = name;
        Desc = desc;
        Path = path;
        OgFile = ogFile;
    }

    public abstract IEnumerable<Result> Evaluate(IEnumerable<IFile> files);
    protected abstract void DeserializeParams();
    
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