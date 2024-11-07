using Core.Evaluation;
using Core.FileHandling;

namespace Core.Questions;

public interface IQuestion<in TFile> where TFile : IFile {
    string Description { get; }
    IEnumerable<Result> Evaluate(IEnumerable<TFile> files);
}
