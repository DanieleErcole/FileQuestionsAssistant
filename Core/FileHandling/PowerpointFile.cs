using Core.Utils.Errors;
using DocumentFormat.OpenXml.Packaging;
using ApplicationException = System.ApplicationException;

namespace Core.FileHandling;

public class PowerpointFile : IFile {

    private readonly PresentationDocument _doc;
    
    public string Name { get; }

    public PowerpointFile(string name, Stream file) {
        Name = name;
        try {
            _doc = PresentationDocument.Open(file, false);
        } catch (Exception e) when (e is not ApplicationException) {
            throw new FileError(Name, e);
        }
    }
    
    public static implicit operator PowerpointFile(byte[] raw) {
        return new PowerpointFile("in_memory", new MemoryStream(raw));
    }
    
    public void Dispose() {
        _doc.Dispose();
        GC.SuppressFinalize(this);
    }
    
}