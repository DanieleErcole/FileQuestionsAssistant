using Core.Evaluation;
using Core.FileHandling;

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

public enum QuestionType {
    CreateStyleQuestion,
}

public class QuestionData {

    public QuestionType Type { get; internal set; }
    public required string Name { get; init; }
    public string? Desc { get; init; }
    
    public required byte[] OgFile { get; init; }
    public required Dictionary<string, object?> Params { get; init; }

}

public abstract class AbstractQuestion(QuestionData data) : IQuestion {
    
    public string Name => Data.Name;
    public string? Desc => Data.Desc;
    protected byte[] OgFile => Data.OgFile;
    protected Dictionary<string, object?> Params => Data.Params;

    public QuestionData Data { get; } = data;

    protected AbstractQuestion(string name, string? desc, string ogFile) : this(new QuestionData {
        Name = name,
        Desc = desc,
        OgFile = Convert.FromBase64String(ogFile),
        Params = new Dictionary<string, object?>()
    }) {}

    public abstract IEnumerable<Result> Evaluate(IEnumerable<IFile> files);

}