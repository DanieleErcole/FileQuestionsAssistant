using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using Core.Utils.Converters;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = System.Drawing.Color;

namespace Core.Questions.Word;

[method: JsonConstructor]
public class CreateStyleQuestion(
        string path,
        string name,
        string? desc,
        MemoryFile ogFile,
        string styleName,
        string? baseStyleName = null,
        string? fontName = null,
        int? fontSize = null,
        Color? color = null,
        Alignment? alignment = null
    ) : AbstractQuestion(path, name, desc, ogFile) {
    
    public string StyleName { get; } = styleName;
    public string? BaseStyleName { get; } = baseStyleName;
    public string? FontName { get; } = fontName;
    public int? FontSize { get; } = fontSize; 
    public Color? color { get; } = color;
    public Alignment? Alignment { get; } = alignment;

    protected override Dictionary<string, object?> ParamsToDict() => new() {
        ["styleName"] = StyleName,
        ["baseStyleName"] = BaseStyleName,
        ["fontName"] = FontName,
        ["fontSize"] = FontSize,
        ["color"] = color,
        ["alignment"] = Alignment
    };

    private static Dictionary<string, object?> StyleToDict(Style s) {
        var rgb = s.StyleRunProperties?.Color?.Val?.Value?.HexStringToRgb();
        var fs = int.Parse(s.StyleRunProperties?.FontSize?.Val?.Value ?? "-1");

        return new Dictionary<string, object?> {
            ["styleName"] = s.StyleName?.Val?.Value,
            ["baseStyleName"] = s.BasedOn?.Val?.Value,
            ["fontName"] = s.StyleRunProperties?.RunFonts?.Ascii?.Value,
            ["fontSize"] = fs == -1 ? null : fs / 2, // The font size is stored in half-points (1/144 of an inch)
            ["color"] = rgb is null ? null : Color.FromArgb(rgb.Value.Item1, rgb.Value.Item2, rgb.Value.Item3),
            ["alignment"] = AlignmentHelper.FromFileString(s.StyleParagraphProperties?.Justification?.Val?.InnerText)
        };
    }
    
    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) => 
        files.OfType<WordFile>().Select(file => {
            var fileStyles = file.Styles
                .Where(s => s.Type?.InnerText == "paragraph")
                .Select(StyleToDict)
                .ToList();
            var matchedStyle = fileStyles.FirstOrDefault(s => 
                s.Get<string>("styleName") == StyleName && 
                (BaseStyleName is null || s.Get<string?>("baseStyleName") == BaseStyleName) &&
                (FontName is null || s.Get<string?>("fontName") == FontName) &&
                (FontSize is null || s.Get<int?>("fontSize") == FontSize) &&
                (color is null || s.Get<Color?>("color") == color) &&
                (Alignment is null || s.Get<Alignment?>("alignment") == Alignment));

            var pars = ParamsToDict();
            if (matchedStyle is not null)
                return new Result(pars, [], true);
            
            using WordFile ogFile = OgFile;
            var ogStyles = ogFile.Styles.Select(StyleToDict).ToList();

            var diff = fileStyles
                .Where(s => ogStyles.All(x =>
                    s.Get<string>("styleName") != x.Get<string>("styleName") ||
                    s.Get<string?>("baseStyleName") != x.Get<string?>("baseStyleName") ||
                    s.Get<string?>("fontName") != x.Get<string?>("fontName") ||
                    s.Get<int?>("fontSize") != x.Get<int?>("fontSize") ||
                    s.Get<Color?>("color") != x.Get<Color?>("color") ||
                    s.Get<Alignment?>("alignment") != x.Get<Alignment?>("alignment")
                ));
            return new Result(pars, diff.ToList(), false);
        });
    
}