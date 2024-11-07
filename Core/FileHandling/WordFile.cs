using Core.Utils.Errors;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace Core.FileHandling; 

public class WordFile : IFile {

    private readonly WordprocessingDocument _doc;

    public MainDocumentPart MainDoc { get; }

    public IEnumerable<Style> Styles {
        get {
            try {
                if (MainDoc.StylesWithEffectsPart is { } s)
                    return s.Styles?.Elements<Style>() ?? throw new InvalidFileFormat();
                return MainDoc.StyleDefinitionsPart?.Styles?.Elements<Style>() ?? throw new InvalidFileFormat();
            } catch (Exception e) when (e is not ApplicationException) {
                throw new FileError(e);
            }
        }
    }

    public WordFile(Stream file) {
        try {
            _doc = WordprocessingDocument.Open(file, false);
            MainDoc = _doc.MainDocumentPart ?? throw new InvalidFileFormat();
        } catch (Exception e) when (e is not ApplicationException) {
            throw new FileError(e);
        }
    }

    public void Dispose() {
        _doc.Dispose();
        GC.SuppressFinalize(this);
    }

}