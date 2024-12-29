using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using DocumentFormat.OpenXml.Presentation;

namespace Core.Questions.Powerpoint;

public enum Origin {
    SlideCenter,
    TopLeftCorner,
}

public class ImageInsertQuestion : AbstractQuestion {

    public ImageInsertQuestion(string path, string name, string? desc, MemoryFile ogFile, MemoryFile image, double? x, double? y, double? width, double? height, 
        Origin? vO, Origin? hO) : base(path, name, desc, ogFile) {

        if ((hO is not null && x is null) || (vO is not null && y is null))
            throw new ArgumentException("Origin must be set when setting horizontal or vertical position and vice versa");
        
        Params.Add("image", image);
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
                "image" => jsonEl?.Deserialize<MemoryFile>(),
                "x" => jsonEl?.Deserialize<double?>(),
                "y" => jsonEl?.Deserialize<double?>(),
                "width" => jsonEl?.Deserialize<double?>(),
                "height" => jsonEl?.Deserialize<double?>(),
                "vOrigin" or "hOrigin" => jsonEl?.Deserialize<Origin?>(),
                _ => throw new JsonException()
            };
        }
    }
    
    private static Dictionary<string, object?> PictureToDict(PowerpointFile file, Picture p) {
        var shapeProps = p.ShapeProperties;
        return new Dictionary<string, object?> {
            ["image"] = file.GetImageFromPic(p),
            ["x"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Offset?.X?.Value),
            ["y"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Offset?.Y?.Value),
            ["width"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Extents?.Cx?.Value),
            ["height"] = EMUsHelper.ToCentimeters(shapeProps?.Transform2D?.Extents?.Cy?.Value),
            ["vOrigin"] = Origin.TopLeftCorner,
            ["hOrigin"] = Origin.TopLeftCorner
        };
    }

    //TODO: Non va è impossibile che un'immagine sia uguale a quella messa in una presentazione, trovare un modo di decidere su cosa devo confrontarle
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
                
                if (Params.Get<Origin?>("hOrigin") is Origin.SlideCenter)
                    x += slideWidth / 2;
                if (Params.Get<Origin?>("vOrigin") is Origin.SlideCenter)
                    y += slideHeight / 2;

                var fileImg = file.GetImageFromPic(picture);
                return Params.Get<MemoryFile>("image")!.Equals(fileImg) &&
                       (x is null || fileX.DoubleEquals(x)) &&
                       (y is null || fileY.DoubleEquals(y)) &&
                       (height is null || fileWidth.DoubleEquals(width)) &&
                       (height is null || fileHeight.DoubleEquals(height));
            });
            
            if (matchedStyle is not null) 
                return new Result(Params, [], true);

            PowerpointFile ogFile = OgFile;
            var ogPictures = ogFile.Pictures.Select(p => PictureToDict(ogFile, p));

            var diff = file.Pictures
                .Select(p => PictureToDict(file, p))
                .Where(p => ogPictures.All(i => {
                    var img = p.Get<MemoryFile>("image");
                    return (img is not null && img.Equals(i.Get<MemoryFile>("image"))) ||
                           !p.Get<double?>("x").DoubleEquals(i.Get<double?>("x")) ||
                           !p.Get<double?>("y").DoubleEquals(i.Get<double?>("y")) ||
                           !p.Get<double?>("width").DoubleEquals(i.Get<double?>("width")) ||
                           !p.Get<double?>("height").DoubleEquals(i.Get<double?>("height")) ||
                           p.Get<Origin?>("hOrigin") != i.Get<Origin?>("hOrigin") ||
                           p.Get<Origin?>("vOrigin") != i.Get<Origin?>("vOrigin");
                }));
            return new Result(Params, diff.ToList(), false);
        });

}