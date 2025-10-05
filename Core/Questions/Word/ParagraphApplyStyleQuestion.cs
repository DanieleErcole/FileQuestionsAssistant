using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;

namespace Core.Questions.Word;

[method: JsonConstructor]
public class ParagraphApplyStyleQuestion(string path, string name, string? desc, MemoryFile ogFile, string styleName)
    : AbstractQuestion(path, name, desc, ogFile) {

    public string StyleName { get; } = styleName;
    
    protected override Dictionary<string, object?> ParamsToDict() => new() {
        ["styleName"] = StyleName
    };

    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) =>
        files.OfType<WordFile>().Select(file => {
            var matchedParagraph = file.Paragraphs.FirstOrDefault(p => p.ParagraphProperties?.ParagraphStyleId?.Val?.Value == StyleName);
            return new Result(ParamsToDict(), [], matchedParagraph is not null);
        });

}