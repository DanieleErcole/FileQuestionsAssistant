using Core.Utils.Errors;
using Core.Utils;
using Core.Utils.Converters;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Picture = DocumentFormat.OpenXml.Presentation.Picture;
using Shape = Core.Utils.Shape;

namespace Core.FileHandling;

public class PowerpointFile : IFile {

    private readonly PresentationDocument _doc;
    private readonly PresentationPart _mainDoc;
    
    public string Name { get; }
    public string Path { get; }
    public Presentation Presentation => _mainDoc.Presentation;

    private Shape ElementToShape(OpenXmlElement parent, Transform2D? transform) {
        var name = parent switch {
            DocumentFormat.OpenXml.Presentation.Shape sp => sp.NonVisualShapeProperties?.NonVisualDrawingProperties?.Name?.Value ?? throw new InvalidFileFormat(Name),
            Picture pic => pic.NonVisualPictureProperties?.NonVisualDrawingProperties?.Name?.Value ?? throw new InvalidFileFormat(Name),
            _ => throw new InvalidOperationException("Invalid element")
        };
        
        return new Shape(name,
            EMUsHelper.ToCentimeters(transform?.Offset?.X?.Value),
            EMUsHelper.ToCentimeters(transform?.Offset?.Y?.Value),
            EMUsHelper.ToCentimeters(transform?.Extents?.Cx?.Value),
            EMUsHelper.ToCentimeters(transform?.Extents?.Cy?.Value),
            DegreesHelper.ToDegrees(transform?.Rotation?.Value),
            transform?.HorizontalFlip?.Value ?? false,
            transform?.VerticalFlip?.Value ?? false
        );
    }

    public IEnumerable<Shape> Shapes {
        get {
            try {
                var slides = _mainDoc.Presentation.PresentationPart?.SlideParts.ToList() ?? throw new InvalidFileFormat(Name);
                var pictures = slides
                    .SelectMany(s => s.Slide.Descendants<Picture>())
                    .Select(p => ElementToShape(p, p.ShapeProperties?.Transform2D));
                var shapes = slides
                    .SelectMany(s => s.Slide.Descendants<DocumentFormat.OpenXml.Presentation.Shape>())
                    .Select(sp => ElementToShape(sp, sp.ShapeProperties?.Transform2D)).ToList();
                return pictures.Concat(shapes);
            } catch (Exception e) when (e is not FileError) {
                throw new FileError(Name, e);
            }
        }
    }

    public PowerpointFile(string name, Stream file) {
        Name = name;
        Path = file is FileStream fs ? fs.Name : Name;
        try {
            _doc = PresentationDocument.Open(file, false);
            _mainDoc = _doc.PresentationPart ?? throw new InvalidFileFormat(Name);
        } catch (Exception e) when (e is not FileError) {
            throw new FileError(Name, e);
        }
    }
    
    public static implicit operator PowerpointFile(MemoryFile inMem) {
        return new PowerpointFile(inMem.Name, new MemoryStream(inMem.Data));
    }
    
    public void Dispose() {
        _doc.Dispose();
        GC.SuppressFinalize(this);
    }
    
    ~PowerpointFile() => Dispose();
    
}