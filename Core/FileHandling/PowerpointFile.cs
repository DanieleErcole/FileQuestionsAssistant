using Core.Utils.Errors;
using System.Linq;
using Core.Utils;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using ApplicationException = System.ApplicationException;

namespace Core.FileHandling;

public class PowerpointFile : IFile {

    private readonly PresentationDocument _doc;
    private readonly PresentationPart _mainDoc;
    
    public string Name { get; }
    public Presentation Presentation => _mainDoc.Presentation;

    public IEnumerable<Picture> Pictures {
        get {
            try {
                var slides = _mainDoc.Presentation.PresentationPart?.SlideParts ?? throw new InvalidFileFormat(Name);
                return slides.SelectMany(s => s.Slide.Descendants<Picture>());
            } catch (Exception e) when (e is not ApplicationException) {
                throw new FileError(Name, e);
            }
        }
    }

    public PowerpointFile(string name, Stream file) {
        Name = name;
        try {
            _doc = PresentationDocument.Open(file, false);
            _mainDoc = _doc.PresentationPart ?? throw new InvalidFileFormat(Name);
        } catch (Exception e) when (e is not FileError or ApplicationException) {
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
    
    ~PowerpointFile() {
        Dispose();
    }
    
}