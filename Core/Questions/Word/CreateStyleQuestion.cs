using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using Color = System.Drawing.Color;

namespace Core.Questions.Word;

public class CreateStyleQuestion : AbstractQuestion<WordFile> {

    public CreateStyleQuestion(string styleName, string? baseStyleName = null, string? fontName = null, int? fontSize = null, Color? color = null, string? alignment = null) {
        _params.Add("styleName", styleName);
        _params.Add("baseStyleName", baseStyleName);
        _params.Add("fontName", fontName);
        _params.Add("fontSize", fontSize);
        _params.Add("color", color);
        _params.Add("alignment", alignment);
    }

    public override IEnumerable<Result> Evaluate(IEnumerable<WordFile> files) => 
        files.Select(file => {
            var baseStyleName = _params.Get<string>("baseStyleName");
            var fontName = _params.Get<string>("fontName");
            var fontSize = _params.Get<int?>("fontSize");
            var color = _params.Get<Color?>("color");
            var alignment = _params.Get<string>("alignment");
            
            var matchedStyle = file.Styles.FirstOrDefault(s => {
                if (s.StyleRunProperties is null) return false;
                
                var styleProps = s.StyleRunProperties;
                var (r, g, b) = styleProps.Color?.Val?.Value?.HexStringToRgb() ?? (-1, -1, -1);
                
                return s.StyleName?.Val?.Value == _params.Get<string>("styleName") && 
                       (baseStyleName is null || s.BasedOn?.Val?.Value == baseStyleName) &&
                       (fontName is null || styleProps.RunFonts?.Ascii?.Value == fontName) &&
                       (fontSize is null || int.Parse(styleProps.FontSize?.Val?.Value ?? "-1") == fontSize * 2) && // The font size is stored in half-points (1/144 of an inch)
                       (color is null || Color.FromArgb(r, g, b) == color) &&
                       (alignment is null || s.StyleParagraphProperties?.Justification?.Val?.InnerText == alignment);
            });

            var stylesFromFile = file.Styles
                .Where(s => s.CustomStyle ?? false)
                .Select(s => {
                    var rgb = s.StyleRunProperties?.Color?.Val?.Value?.HexStringToRgb();
                    var fs = int.Parse(s.StyleRunProperties?.FontSize?.Val?.Value ?? "-1");
                    
                    return new Dictionary<string, object?> {
                        ["styleName"] = s.StyleName?.Val,
                        ["baseStyleName"] = s.BasedOn?.Val,
                        ["fontName"] = s.StyleRunProperties?.RunFonts?.Ascii?.Value,
                        ["fontSize"] = fs == -1 ? null : fs / 2,
                        ["color"] = rgb is null ? null : Color.FromArgb(rgb.Value.Item1, rgb.Value.Item2, rgb.Value.Item3),
                        ["alignment"] = s.StyleParagraphProperties?.Justification?.Val?.InnerText
                    };
                }).ToList();

            return new Result(_params, stylesFromFile, matchedStyle is not null);
        });

}