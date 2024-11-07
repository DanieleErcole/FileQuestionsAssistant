using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using Color = System.Drawing.Color;

namespace Core.Questions.Word;

public class CreateStyleQuestion : AbstractQuestion<WordFile> {

    public override string Description => "Checks if a style with the specified properties exists in the document";

    public CreateStyleQuestion(string styleName, string baseStyleName, string fontName, int fontSize, Color color, string alignment) {
        _params.Add("styleName", styleName);
        _params.Add("baseStyleName", baseStyleName);
        _params.Add("fontName", fontName);
        _params.Add("fontSize", fontSize);
        _params.Add("color", color);
        _params.Add("alignment", alignment);
    }

    public override IEnumerable<Result> Evaluate(IEnumerable<WordFile> files) => 
        files.Select(file => {
            var matchedStyle = file.Styles.FirstOrDefault(s => {
                var styleProp = s?.StyleRunProperties;
                var (r, g, b) = styleProp?.Color?.Val?.Value?.HexStringToRgb() ?? (-1, -1, -1);

                return s?.StyleName?.Val?.Value == _params.Get<string>("styleName") && 
                       s.BasedOn?.Val?.Value == _params.Get<string>("baseStyleName") && 
                       styleProp?.RunFonts?.Ascii?.Value == _params.Get<string>("fontName") && 
                       int.Parse(styleProp.FontSize?.Val?.Value ?? "-1") == _params.Get<int>("fontSize") * 2 && // The font size is stored in half-points (1/144 of an inch)
                       Color.FromArgb(r, g, b) == _params.Get<Color>("color") && 
                       s.StyleParagraphProperties?.Justification?.Val?.InnerText == _params.Get<string>("alignment");
            } , null);
            Dictionary<string, object>? fileParams = null;

            if (matchedStyle is not null) {
                var (r, g, b) = matchedStyle.StyleRunProperties!.Color!.Val!.Value!.HexStringToRgb()!;
                fileParams = new Dictionary<string, object> {
                    ["styleName"] = matchedStyle.StyleName!.Val!,
                    ["baseStyleName"] = matchedStyle.BasedOn!.Val!,
                    ["fontName"] = matchedStyle.StyleRunProperties!.RunFonts!.Ascii!.Value!,
                    ["fontSize"] = int.Parse(matchedStyle.StyleRunProperties!.FontSize!.Val!.Value!) / 2,
                    ["color"] = Color.FromArgb(r, g, b),
                    ["alignment"] = matchedStyle.StyleParagraphProperties!.Justification!.Val!.InnerText!
                };
            }

            return new Result(_params, fileParams);
        });

}