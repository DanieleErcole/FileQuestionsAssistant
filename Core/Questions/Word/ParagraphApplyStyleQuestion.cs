using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Core.Questions.Word;

public class ParagraphApplyStyleQuestion : AbstractQuestion {
    
    public ParagraphApplyStyleQuestion(string path, string name, string? desc, MemoryFile ogFile, string styleName) : base(path, name, desc, ogFile) {
        Params.Add("styleName", styleName);
    }
    
    [JsonConstructor]
    public ParagraphApplyStyleQuestion(string name, string desc, string path, MemoryFile ogFile, Dictionary<string, object?> Params) 
        : base(name, desc, ogFile, Params) {}

    protected override void DeserializeParams() {
        foreach (var (k, v) in Params) {
            var jsonEl = (JsonElement?) v;
            Params[k] = k switch {
                "styleName" => jsonEl?.Deserialize<string>(),
                _ => throw new JsonException()
            };
        }
    }

    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) =>
        files.OfType<WordFile>().Select(file => {
            var styleName = Params.Get<string>("styleName");
            var matchedParagraph = file.Paragraphs.FirstOrDefault(p => {
                var pProperties = p.ParagraphProperties;
                if (pProperties is null) return false;
                
                return pProperties.ParagraphStyleId?.Val?.Value == styleName;
            });
            
            return new Result(Params, [], matchedParagraph is not null);
        });

}