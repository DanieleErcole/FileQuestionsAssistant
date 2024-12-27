using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using Core.Utils.Errors;
using DocumentFormat.OpenXml.Presentation;

namespace Core.Questions.Powerpoint;

public class ImageInsertQuestion : AbstractQuestion {

    public enum Origin {
        SlideCenter,
        TopLeftCorner,
    }

    public ImageInsertQuestion(string path, string name, string? desc, byte[] ogFile, string imageName, double? x, double? y, double? width, double? height, Origin? o) 
        : base(path, name, desc, ogFile) {

        if (o is not null && x is null && y is null)
            throw new ArgumentException("Origin must be set when setting horizontal or vertical position and vice versa");
        
        var (xCm, yCm) = (EMUsHelper.ToCentimeters(x), EMUsHelper.ToCentimeters(y));
        var (widthCm, heightCm) = (EMUsHelper.ToCentimeters(width), EMUsHelper.ToCentimeters(height));
        Params.Add("imageName", imageName);
        Params.Add("x", xCm);
        Params.Add("y", yCm);
        Params.Add("width", widthCm);
        Params.Add("height", heightCm);
        Params.Add("origin", o);
    }
    
    [JsonConstructor]
    public ImageInsertQuestion(string name, string desc, byte[] ogFile, Dictionary<string, object?> Params) 
        : base(name, desc, ogFile, Params) { }

    protected override void DeserializeParams() {
        foreach (var (k, v) in Params) {
            var jsonEl = (JsonElement?) v;
            Params[k] = k switch {
                "imageName" => jsonEl?.Deserialize<string>(),
                "x" => jsonEl?.Deserialize<double?>(),
                "y" => jsonEl?.Deserialize<double?>(),
                "width" => jsonEl?.Deserialize<double?>(),
                "height" => jsonEl?.Deserialize<double?>(),
                "origin" => jsonEl?.Deserialize<Origin?>(),
                _ => throw new JsonException()
            };
        }
    }
    
    private static Dictionary<string, object?> PictureToDict(Picture p) {
        var shapeProps = p.ShapeProperties;
        var nvProps = p.NonVisualPictureProperties;
        
        return new Dictionary<string, object?> {
            ["imageName"] = nvProps?.NonVisualDrawingProperties?.Name?.Value,
            ["x"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Offset?.X?.Value),
            ["y"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Offset?.Y?.Value),
            ["width"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Extents?.Cx?.Value),
            ["height"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Extents?.Cy?.Value),
            ["origin"] = Origin.TopLeftCorner
        };
    }

    //TODO: testarlo
    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) =>
        files.OfType<PowerpointFile>().Select(file => {
            var x = Params.Get<double?>("x");
            var y = Params.Get<double?>("y");
            var width = Params.Get<double?>("width");
            var height = Params.Get<double?>("height");

            var matchedStyle = file.Pictures.FirstOrDefault(image => {
                if (image.ShapeProperties is not { } shapeProps || image.NonVisualPictureProperties is not {} nvProps
                    || file.Presentation.SlideSize is not { Cx.Value: var slideWidth, Cy.Value: var slideHeight }) 
                    return false;
                var fileX = EMUsHelper.ToCentimeters(shapeProps.Transform2D?.Offset?.X?.Value);
                var fileY = EMUsHelper.ToCentimeters(shapeProps.Transform2D?.Offset?.Y?.Value);
                var fileWidth = EMUsHelper.ToCentimeters(shapeProps.Transform2D?.Extents?.Cx?.Value);
                var fileHeight = EMUsHelper.ToCentimeters(shapeProps.Transform2D?.Extents?.Cy?.Value);
                
                if (Params.Get<Origin?>("origin") is Origin.SlideCenter) {
                    x += slideWidth / 2;
                    y += slideHeight / 2;
                }
                
                return nvProps.NonVisualDrawingProperties?.Name?.Value == Params.Get<string>("imageName") &&
                       (x is null || fileX.DoubleEquals(x)) &&
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
                    p.Get<string>("imageName") != i.Get<string>("imageName") ||
                    !p.Get<double?>("x").DoubleEquals(i.Get<double?>("x")) ||
                    !p.Get<double?>("y").DoubleEquals(i.Get<double?>("y")) ||
                    !p.Get<double?>("width").DoubleEquals(i.Get<double?>("width")) ||
                    !p.Get<double?>("height").DoubleEquals(i.Get<double?>("height")) ||
                    p.Get<Origin?>("origin") != i.Get<Origin?>("origin")
                ));
            return new Result(Params, diff.ToList(), false);
        });

}