using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;

namespace Core.Questions.Powerpoint;

public class ImageInsertQuestion : AbstractQuestion {

    public ImageInsertQuestion(string path, string name, string? desc, byte[] ogFile, double x, double y, double width, double height) : base(path, name, desc, ogFile) {
        var (xCm, yCm) = EMUsHelper.ToCentimeters(x, y);
        var(widthCm, heightCm) = EMUsHelper.ToCentimeters(width, height);
        Params.Add("x", xCm);
        Params.Add("y", yCm);
        Params.Add("width", widthCm);
        Params.Add("height", heightCm);
    }
    
    [JsonConstructor]
    public ImageInsertQuestion(string name, string desc, byte[] ogFile, Dictionary<string, object?> Params) 
        : base(name, desc, ogFile, Params) { }

    protected override void DeserializeParams() {
        foreach (var (k, v) in Params) {
            var jsonEl = (JsonElement?) v;
            Params[k] = k switch {
                "x" => jsonEl?.Deserialize<int>(),
                "y" => jsonEl?.Deserialize<int>(),
                "width" => jsonEl?.Deserialize<int>(),
                "height" => jsonEl?.Deserialize<int>(),
                _ => throw new JsonException()
            };
        }
    }

    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) {
        throw new NotImplementedException();
    }

}