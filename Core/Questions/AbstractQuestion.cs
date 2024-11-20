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

public abstract class AbstractQuestion<TFile> : IQuestion<TFile> where TFile : IFile {

    protected readonly Dictionary<string, object?> _params = new();

    public abstract IEnumerable<Result> Evaluate(IEnumerable<TFile> files);

}