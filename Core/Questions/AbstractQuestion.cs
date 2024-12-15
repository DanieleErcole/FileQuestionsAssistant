using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Word;

namespace Core.Questions;

public static class ParamsExtensions {
    public static T? Get<T>(this Dictionary<string, object?> d, string index) {
        if (d[index] is { } el && !el.GetType().IsAssignableTo(typeof(T)))
            throw new InvalidCastException($"Cannot cast type {el.GetType()} to {typeof(T)}");

        if (d[index] is null) 
            return default;
        
        return (T?) d[index];
    }
}

//TODO: add other question types
[JsonDerivedType(typeof(CreateStyleQuestion), typeDiscriminator: 0)]
public abstract class AbstractQuestion : IQuestion {
    
    public string Name { get; set; }
    public string? Desc { get; set; }
    public byte[] OgFile { get; set; }
    
    [JsonIgnore]
    public string Path { get; set; } // The path will be initialized when creating the question from code and after deserializing it, it will be always initialized

    [JsonInclude]
    public Dictionary<string, object?> Params = new();

    [JsonConstructor]
    public AbstractQuestion(string name, string desc, byte[] ogFile, Dictionary<string, object?> Params) {
        Name = name;
        Desc = desc;
        OgFile = ogFile;
        this.Params = Params;
        // The virtual method changes the Params initialised above,
        // ìgnore the warning
        DeserializeParams();
    }

    protected AbstractQuestion(string path, string name, string? desc, byte[] ogFile) {
        Name = name;
        Desc = desc;
        Path = path;
        OgFile = ogFile;
    }

    public abstract IEnumerable<Result> Evaluate(IEnumerable<IFile> files);
    protected abstract void DeserializeParams();

}