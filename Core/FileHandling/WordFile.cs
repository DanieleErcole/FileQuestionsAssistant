using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Core.FileHandling; 

public class WordFile : IFile {

    public string FileFormat { get; } = "docx";

    private readonly WordprocessingDocument _doc;

    public MainDocumentPart MainDoc { get; }

    public IEnumerable<Style> Styles {
        get {
            if (MainDoc.StylesWithEffectsPart is { } s)
                return s.Styles?.Elements<Style>() ?? throw new ArgumentException();
            return MainDoc.StyleDefinitionsPart?.Styles?.Elements<Style>() ?? throw new ArgumentException();
        }
    }

    public WordFile(Stream file) {
        _doc = WordprocessingDocument.Open(file, false);
        MainDoc = _doc.MainDocumentPart ?? throw new ArgumentException();
    }

    public void Dispose() {
        _doc.Dispose();
    }

}