using Core.Evaluation;
using Core.FileHandling;

namespace Core.Questions;

public static class ParamsExtensions {
    public static T Get<T>(this Dictionary<string, object> d, string index) where T : notnull {
        if (!d[index].GetType().IsAssignableTo(typeof(T)))
            throw new InvalidCastException($"Cannot cast type {d[index].GetType()} to {typeof(T)}");
        return (T) d[index];
    }
}

public abstract class AbstractQuestion<TFile> : IQuestion<TFile> where TFile : IFile {

    protected readonly Dictionary<string, object> _params = new();

    public abstract string Description { get; }

    public abstract IEnumerable<Result> Evaluate(IEnumerable<TFile> files);

}