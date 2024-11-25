using Core.Utils.Errors;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace Core.FileHandling; 

public class WordFile : IFile {

    private readonly WordprocessingDocument _doc;

    public string Name { get; }
    public MainDocumentPart MainDoc { get; }

    public IEnumerable<Style> Styles {
        get {
            try {
                if (MainDoc.StylesWithEffectsPart is { } s)
                    return s.Styles?.Elements<Style>() ?? throw new InvalidFileFormat(Name);
                return MainDoc.StyleDefinitionsPart?.Styles?.Elements<Style>() ?? throw new InvalidFileFormat(Name);
            } catch (Exception e) when (e is not ApplicationException) {
                throw new FileError(Name, e);
            }
        }
    }

    public static WordFile FromPath(string path) {
        var file = File.Open(path, FileMode.Open, FileAccess.Read);
        return new WordFile(file.Name, file);
    }
    
    public WordFile(string name, Stream file) {
        Name = name;
        try {
            _doc = WordprocessingDocument.Open(file, false);
            MainDoc = _doc.MainDocumentPart ?? throw new InvalidFileFormat(Name);
        } catch (Exception e) when (e is not ApplicationException) {
            throw new FileError(Name, e);
        }
    }

    public void Dispose() {
        _doc.Dispose();
        GC.SuppressFinalize(this);
    }

}