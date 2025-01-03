using Core.Utils;
using Core.Utils.Errors;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace Core.FileHandling; 

public class WordFile : IFile {

    private readonly WordprocessingDocument _doc;
    private readonly MainDocumentPart _mainDoc;
    
    public string Name { get; }
    public string Path { get; }

    public IEnumerable<Style> Styles {
        get {
            try {
                if (_mainDoc.StylesWithEffectsPart is { } s)
                    return s.Styles?.Elements<Style>() ?? throw new InvalidFileFormat(Name);
                return _mainDoc.StyleDefinitionsPart?.Styles?.Elements<Style>() ?? throw new InvalidFileFormat(Name);
            } catch (Exception e) when (e is not ApplicationException) {
                throw new FileError(Name, e);
            }
        }
    }

    public IEnumerable<Font> Fonts {
        get {
            try {
                return _mainDoc.FontTablePart?.Fonts.Elements<Font>() ?? throw new InvalidFileFormat(Name);
            } catch (Exception e) when (e is not ApplicationException) {
                throw new FileError(Name, e);
            }
        }
    }

    public IEnumerable<Paragraph> Paragraphs {
        get {
            try {
                return _mainDoc.Document.Body?.Elements<Paragraph>() ?? throw new InvalidFileFormat(Name);
            } catch (Exception e) when (e is not ApplicationException) {
                throw new FileError(Name, e);
            }
        }
    }
    
    public WordFile(string name, Stream file) {
        Name = name;
        Path = file is FileStream fs ? fs.Name : Name;
        try {
            _doc = WordprocessingDocument.Open(file, false);
            _mainDoc = _doc.MainDocumentPart ?? throw new InvalidFileFormat(Name);
        } catch (Exception e) when (e is not ApplicationException) {
            throw new FileError(Name, e);
        }
    }
    
    public static implicit operator WordFile(MemoryFile inMem) {
        return new WordFile(inMem.Name, new MemoryStream(inMem.Data));
    }

    public void Dispose() {
        _doc.Dispose();
        GC.SuppressFinalize(this);
    }
    
    ~WordFile() {
        Dispose();
    }
    
}