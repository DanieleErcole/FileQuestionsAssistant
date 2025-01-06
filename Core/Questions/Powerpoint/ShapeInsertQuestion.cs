using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using Core.Utils.Converters;
using Origin = Core.Utils.Origin;
using Shape = Core.Utils.Shape;

namespace Core.Questions.Powerpoint;

public class ShapeInsertQuestion : AbstractQuestion {

    public ShapeInsertQuestion(string path, string name, string? desc, MemoryFile ogFile, double? x, double? y, double? width, double? height, int? rotation,
        Origin vO, Origin hO, bool flipV = false, bool flipH = false) : base(path, name, desc, ogFile) {
        
        if (x is null && y is null && width is null && height is null && rotation is null)
            throw new ArgumentException("Parameters must be set");
        
        Params.Add("x", x);
        Params.Add("y", y);
        Params.Add("width", width);
        Params.Add("height", height);
        Params.Add("flipH", flipH);
        Params.Add("flipV", flipV);
        Params.Add("rotation", rotation);
        Params.Add("hOrigin", hO);
        Params.Add("vOrigin", vO);
    }
    
    [JsonConstructor]
    public ShapeInsertQuestion(string name, string desc, MemoryFile ogFile, Dictionary<string, object?> Params) 
        : base(name, desc, ogFile, Params) { }

    protected override void DeserializeParams() {
        foreach (var (k, v) in Params) {
            var jsonEl = (JsonElement?) v;
            Params[k] = k switch {
                "x" or "y" or "width" or "height" => jsonEl?.Deserialize<double?>(),
                "rotation" => jsonEl?.Deserialize<int?>(),
                "flipH" or "flipV" => jsonEl?.Deserialize<bool>(),
                "vOrigin" or "hOrigin" => jsonEl?.Deserialize<Origin>(),
                _ => throw new JsonException()
            };
        }
    }
    
    private static Dictionary<string, object?> ShapeToDict(Shape sp) => new() {
        ["name"] = sp.Name,
        ["x"] = sp.X,
        ["y"] = sp.Y,
        ["width"] = sp.Width,
        ["height"] = sp.Height,
        ["rotation"] = sp.Rotation,
        ["flipH"] = sp.HFlip,
        ["flipV"] = sp.VFlip,
        ["vOrigin"] = Origin.TopLeftCorner,
        ["hOrigin"] = Origin.TopLeftCorner
    };
    
    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) =>
        files.OfType<PowerpointFile>().Select(file => {
            var width = Params.Get<double?>("width");
            var height = Params.Get<double?>("height");
            var rotation = Params.Get<int?>("rotation");
            
            var matchedShape = file.Shapes.FirstOrDefault(shape => {
                if (file.Presentation.SlideSize is not { Cx.Value: var slideWidth, Cy.Value: var slideHeight }) 
                    return false;

                double? newX = Params.Get<double?>("x"), newY = Params.Get<double?>("y");
                if (Params.Get<Origin>("hOrigin") is Origin.SlideCenter) 
                    newX += EMUsHelper.ToCentimeters(slideWidth) / 2;
                if (Params.Get<Origin>("vOrigin") is Origin.SlideCenter)
                    newY += EMUsHelper.ToCentimeters(slideHeight) / 2;
                
                return (newX is null || shape.X.DoubleEquals(newX)) &&
                       (newY is null || shape.Y.DoubleEquals(newY)) &&
                       (width is null || shape.Width.DoubleEquals(width)) &&
                       (height is null || shape.Height.DoubleEquals(height)) &&
                       (rotation is null || shape.Rotation == rotation) &&
                       Params.Get<bool>("flipH") == shape.HFlip &&
                       Params.Get<bool>("flipV") == shape.VFlip;
            });
            
            if (matchedShape is not null) 
                return new Result(Params, [], true);

            PowerpointFile ogFile = OgFile;
            var ogShapes = ogFile.Shapes.Select(ShapeToDict);

            var diff = file.Shapes
                .Select(ShapeToDict)
                .Where(p => ogShapes.All(i => 
                    !p.Get<double?>("x").DoubleEquals(i.Get<double?>("x")) ||
                    !p.Get<double?>("y").DoubleEquals(i.Get<double?>("y")) ||
                    !p.Get<double?>("width").DoubleEquals(i.Get<double?>("width")) ||
                    !p.Get<double?>("height").DoubleEquals(i.Get<double?>("height")) ||
                    p.Get<int?>("rotation") != i.Get<int?>("rotation") ||
                    p.Get<bool>("flipH") != i.Get<bool>("flipH") ||
                    p.Get<bool>("flipV") != i.Get<bool>("flipV") ||
                    p.Get<Origin>("hOrigin") != i.Get<Origin>("hOrigin") ||
                    p.Get<Origin>("vOrigin") != i.Get<Origin>("vOrigin")
                ));
            return new Result(Params, diff.ToList(), false);
        });

}