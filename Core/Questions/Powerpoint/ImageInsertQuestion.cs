using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using DocumentFormat.OpenXml.Presentation;

namespace Core.Questions.Powerpoint;

public enum Origin {
    SlideCenter = 0,
    TopLeftCorner = 1,
}

public class ImageInsertQuestion : AbstractQuestion {

    public ImageInsertQuestion(string path, string name, string? desc, MemoryFile ogFile, double? x, double? y, double? width, double? height, Origin vO, Origin hO) 
        : base(path, name, desc, ogFile) {
        
        if (x is null && y is null && width is null && height is null)
            throw new ArgumentException("Parameters must be set");
        
        Params.Add("x", x);
        Params.Add("y", y);
        Params.Add("width", width);
        Params.Add("height", height);
        Params.Add("hOrigin", hO);
        Params.Add("vOrigin", vO);
    }
    
    [JsonConstructor]
    public ImageInsertQuestion(string name, string desc, MemoryFile ogFile, Dictionary<string, object?> Params) 
        : base(name, desc, ogFile, Params) { }

    protected override void DeserializeParams() {
        foreach (var (k, v) in Params) {
            var jsonEl = (JsonElement?) v;
            Params[k] = k switch {
                "x" => jsonEl?.Deserialize<double?>(),
                "y" => jsonEl?.Deserialize<double?>(),
                "width" => jsonEl?.Deserialize<double?>(),
                "height" => jsonEl?.Deserialize<double?>(),
                "vOrigin" or "hOrigin" => jsonEl?.Deserialize<Origin>(),
                _ => throw new JsonException()
            };
        }
    }
    
    private static Dictionary<string, object?> PictureToDict(Picture p) {
        var shapeProps = p.ShapeProperties;
        return new Dictionary<string, object?> {
            ["imageName"] = p.NonVisualPictureProperties?.NonVisualDrawingProperties?.Name?.Value,
            ["x"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Offset?.X?.Value),
            ["y"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Offset?.Y?.Value),
            ["width"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Extents?.Cx?.Value),
            ["height"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Extents?.Cy?.Value),
            ["vOrigin"] = Origin.TopLeftCorner,
            ["hOrigin"] = Origin.TopLeftCorner
        };
    }
    
    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) =>
        files.OfType<PowerpointFile>().Select(file => {
            var x = Params.Get<double?>("x");
            var y = Params.Get<double?>("y");
            var width = Params.Get<double?>("width");
            var height = Params.Get<double?>("height");
            
            var matchedStyle = file.Pictures.FirstOrDefault(picture => {
                if (picture.ShapeProperties is not { } shapeProps 
                    || file.Presentation.SlideSize is not { Cx.Value: var slideWidth, Cy.Value: var slideHeight }) 
                    return false;
                var fileX = EMUsHelper.ToCentimeters(shapeProps.Transform2D?.Offset?.X?.Value);
                var fileY = EMUsHelper.ToCentimeters(shapeProps.Transform2D?.Offset?.Y?.Value);
                var fileWidth = EMUsHelper.ToCentimeters(shapeProps.Transform2D?.Extents?.Cx?.Value);
                var fileHeight = EMUsHelper.ToCentimeters(shapeProps.Transform2D?.Extents?.Cy?.Value);
                
                if (Params.Get<Origin>("hOrigin") is Origin.SlideCenter)
                    x += slideWidth / 2;
                if (Params.Get<Origin>("vOrigin") is Origin.SlideCenter)
                    y += slideHeight / 2;
                
                return (x is null || fileX.DoubleEquals(x)) &&
                       (y is null || fileY.DoubleEquals(y)) &&
                       (height is null || fileWidth.DoubleEquals(width)) &&
                       (height is null || fileHeight.DoubleEquals(height));
            });
            
            if (matchedStyle is not null) 
                return new Result(Params, [], true);

            PowerpointFile ogFile = OgFile;
            var ogPictures = ogFile.Pictures.Select(PictureToDict);

            var diff = file.Pictures
                .Select(PictureToDict)
                .Where(p => ogPictures.All(i => 
                    !p.Get<double?>("x").DoubleEquals(i.Get<double?>("x")) ||
                    !p.Get<double?>("y").DoubleEquals(i.Get<double?>("y")) ||
                    !p.Get<double?>("width").DoubleEquals(i.Get<double?>("width")) ||
                    !p.Get<double?>("height").DoubleEquals(i.Get<double?>("height")) ||
                    p.Get<Origin>("hOrigin") != i.Get<Origin>("hOrigin") ||
                    p.Get<Origin>("vOrigin") != i.Get<Origin>("vOrigin")
                ));
            return new Result(Params, diff.ToList(), false);
        });

}