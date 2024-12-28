using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = System.Drawing.Color;

namespace Core.Questions.Word;

public class CreateStyleQuestion : AbstractQuestion {

    public CreateStyleQuestion(string path, string name, string? desc, MemoryFile ogFile, string styleName, string? baseStyleName = null, 
        string? fontName = null, int? fontSize = null, Color? color = null, string? alignment = null) : base(path, name, desc, ogFile) {
        Params.Add("styleName", styleName);
        Params.Add("baseStyleName", baseStyleName);
        Params.Add("fontName", fontName);
        Params.Add("fontSize", fontSize);
        Params.Add("color", color);
        Params.Add("alignment", alignment);
    }
    
    [JsonConstructor]
    public CreateStyleQuestion(string name, string desc, MemoryFile ogFile, Dictionary<string, object?> Params) 
        : base(name, desc, ogFile, Params) {}
    
    protected override void DeserializeParams() {
        foreach (var (k, v) in Params) {
            var jsonEl = (JsonElement?) v;
            Params[k] = k switch {
                "styleName" => jsonEl?.Deserialize<string>(),
                "baseStyleName" => jsonEl?.Deserialize<string?>(),
                "fontName" => jsonEl?.Deserialize<string?>(),
                "fontSize" => jsonEl?.Deserialize<int?>(),
                "color" => jsonEl?.Deserialize<Color?>(new JsonSerializerOptions {
                    IncludeFields = true,
                    WriteIndented = true,
                    Converters = { new ColorConverter() }
                }),
                "alignment" => jsonEl?.Deserialize<string?>(),
                _ => throw new JsonException()
            };
        }
    }

    private static Dictionary<string, object?> StyleToDict(Style s) {
        var rgb = s.StyleRunProperties?.Color?.Val?.Value?.HexStringToRgb();
        var fs = int.Parse(s.StyleRunProperties?.FontSize?.Val?.Value ?? "-1");

        return new Dictionary<string, object?> {
            ["styleName"] = s.StyleName?.Val?.Value,
            ["baseStyleName"] = s.BasedOn?.Val?.Value,
            ["fontName"] = s.StyleRunProperties?.RunFonts?.Ascii?.Value,
            ["fontSize"] = fs == -1 ? null : fs / 2,
            ["color"] = rgb is null ? null : Color.FromArgb(rgb.Value.Item1, rgb.Value.Item2, rgb.Value.Item3),
            ["alignment"] = s.StyleParagraphProperties?.Justification?.Val?.InnerText
        };
    }
    
    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) => 
        files.OfType<WordFile>().Select(file => {
            var baseStyleName = Params.Get<string?>("baseStyleName");
            var fontName = Params.Get<string?>("fontName");
            var fontSize = Params.Get<int?>("fontSize");
            var color = Params.Get<Color?>("color");
            var alignment = Params.Get<string?>("alignment");
            
            var matchedStyle = file.Styles.FirstOrDefault(s => {
                if (s.StyleRunProperties is not {} styleProps) return false;
                
                var (r, g, b) = styleProps.Color?.Val?.Value?.HexStringToRgb() ?? (-1, -1, -1);
                return s.StyleName?.Val?.Value == Params.Get<string>("styleName") && 
                       (baseStyleName is null || s.BasedOn?.Val?.Value == baseStyleName) &&
                       (fontName is null || styleProps.RunFonts?.Ascii?.Value == fontName) &&
                       (fontSize is null || int.Parse(styleProps.FontSize?.Val?.Value ?? "-1") == fontSize * 2) && // The font size is stored in half-points (1/144 of an inch)
                       (color is null || Color.FromArgb(r, g, b) == color) &&
                       (alignment is null || s.StyleParagraphProperties?.Justification?.Val?.InnerText == alignment);
            });
            
            if (matchedStyle is not null) 
                return new Result(Params, [], true);
            
            WordFile ogFile = OgFile;
            var ogStyles = ogFile.Styles.Select(StyleToDict);

            var diff = file.Styles
                .Where(s => s.Type?.InnerText == "paragraph")
                .Select(StyleToDict)
                .Where(s => ogStyles.All(x =>
                    s.Get<string>("styleName") != x.Get<string>("styleName") ||
                    s.Get<string?>("baseStyleName") != x.Get<string?>("baseStyleName") ||
                    s.Get<string?>("fontName") != x.Get<string?>("fontName") ||
                    s.Get<int?>("fontSize") != x.Get<int?>("fontSize") ||
                    s.Get<Color?>("color") != x.Get<Color?>("color") ||
                    s.Get<string?>("alignment") != x.Get<string?>("alignment")
                ));
            return new Result(Params, diff.ToList(), false);
        });
}