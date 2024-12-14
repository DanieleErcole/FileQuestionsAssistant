using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Core.Questions.Word;

public class ParagraphApplyStyleQuestion : AbstractQuestion {
    
    public ParagraphApplyStyleQuestion(string path, string name, string? desc, byte[] ogFile, string styleName) : base(path, name, desc, ogFile) {
        Params.Add("styleName", styleName);
    }
    
    [JsonConstructor]
    public ParagraphApplyStyleQuestion(string name, string desc, string path, byte[] ogFile, Dictionary<string, object?> Params) 
        : base(name, desc, path, ogFile, Params) {}

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

            if (matchedParagraph is not null)
                return new Result(Params, [], true);

            var parToDict = new Func<Paragraph, Dictionary<string, object?>>(p => new Dictionary<string, object?> {
                ["styleName"] = p.ParagraphProperties?.ParagraphStyleId?.Val?.Value,
            });
            
            WordFile ogFile = OgFile;
            var ogParagraphs = ogFile.Paragraphs.Select(parToDict);
            var diff = file.Paragraphs
                .Select(parToDict)
                .Where(p => ogParagraphs.All(x =>
                    p.Get<string>("styleName") != x.Get<string>("styleName")
                ));
            return new Result(Params, diff.ToList(), false);
        });

}