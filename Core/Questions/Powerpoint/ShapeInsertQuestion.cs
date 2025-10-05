using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils;
using Core.Utils.Converters;
using Origin = Core.Utils.Origin;
using Shape = Core.Utils.Shape;

namespace Core.Questions.Powerpoint;

public class ShapeInsertQuestion : AbstractQuestion {

    public double? X { get; }
    public double? Y { get; }
    public double? Width { get; }
    public double? Height { get; }
    public int? Rotation { get; }
    public Origin HOrigin { get; }
    public Origin VOrigin { get; }
    public bool FlipV { get; }
    public bool FlipH { get; }

    [JsonConstructor]
    public ShapeInsertQuestion(string path, string name, string? desc, MemoryFile ogFile, double? x, double? y, double? width, double? height, int? rotation,
        Origin vO, Origin hO, bool flipV = false, bool flipH = false) : base(path, name, desc, ogFile) {
        
        if (x is null && y is null && width is null && height is null && rotation is null)
            throw new ArgumentException("Parameters must be set");

        X = x;
        Y = y;
        Width = width;
        Height = height;
        Rotation = rotation;
        HOrigin = hO;
        VOrigin = vO;
        FlipV = flipV;
        FlipH = flipH;
    }

    protected override Dictionary<string, object?> ParamsToDict() => new() {
        ["x"] = X,
        ["y"] = Y,
        ["width"] = Width,
        ["height"] = Height,
        ["rotation"] = Rotation,
        ["flipH"] = FlipH,
        ["flipV"] = FlipV,
    };
    
    private static Dictionary<string, object?> ShapeToDict(Shape sp) => new() {
        ["name"] = sp.Name,
        ["x"] = sp.X,
        ["y"] = sp.Y,
        ["width"] = sp.Width,
        ["height"] = sp.Height,
        ["rotation"] = sp.Rotation,
        ["flipH"] = sp.HFlip,
        ["flipV"] = sp.VFlip,
    };
    
    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) =>
        files.OfType<PowerpointFile>().Select(file => {
            var matchedShape = file.Shapes.FirstOrDefault(shape => {
                if (file.Presentation.SlideSize is not { Cx.Value: var slideWidth, Cy.Value: var slideHeight }) 
                    return false;

                double? newX = X, newY = Y;
                if (HOrigin is Origin.SlideCenter) 
                    newX += EMUsHelper.ToCentimeters(slideWidth) / 2;
                if (VOrigin is Origin.SlideCenter)
                    newY += EMUsHelper.ToCentimeters(slideHeight) / 2;
                
                return (newX is null || shape.X.DoubleEquals(newX)) &&
                       (newY is null || shape.Y.DoubleEquals(newY)) &&
                       (Width is null || shape.Width.DoubleEquals(Width)) &&
                       (Height is null || shape.Height.DoubleEquals(Height)) &&
                       (Rotation is null || shape.Rotation == Rotation) &&
                       FlipH == shape.HFlip &&
                       FlipV == shape.VFlip;
            });

            var pars = ParamsToDict();
            if (matchedShape is not null) 
                return new Result(pars, [], true);

            using PowerpointFile ogFile = OgFile;
            var ogShapes = ogFile.Shapes.ToList();
            var diff = file.Shapes
                .Where(sp => ogShapes.All(ogSp => 
                    !sp.X.DoubleEquals(ogSp.X) ||
                    !sp.Y.DoubleEquals(ogSp.Y) ||
                    !sp.Width.DoubleEquals(ogSp.Width) ||
                    !sp.Height.DoubleEquals(ogSp.Height) ||
                    sp.Rotation != ogSp.Rotation ||
                    sp.HFlip != ogSp.HFlip ||
                    sp.VFlip != ogSp.VFlip
                ));
            return new Result(pars, diff.Select(ShapeToDict).ToList(), false);
        });
    
}