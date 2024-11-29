using Core.Evaluation;
using Core.FileHandling;

namespace Core.Questions;

public interface IQuestion {
    public string Name { get; set; }
    public string? Desc { get; set; }
    public string Path { get; set; }
    public byte[] OgFile { get; set; }
    IEnumerable<Result> Evaluate(IEnumerable<IFile> files);
}
