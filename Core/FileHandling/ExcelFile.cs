using Core.Utils.Errors;
using DocumentFormat.OpenXml.Packaging;
using ApplicationException = System.ApplicationException;

namespace Core.FileHandling;

public class ExcelFile : IFile {

    private readonly SpreadsheetDocument _doc;
    
    public string Name { get; }
    public string Path { get; }

    public ExcelFile(string name, Stream file) {
        Name = name;
        Path = file is FileStream fs ? fs.Name : Name;
        try {
            _doc = SpreadsheetDocument.Open(file, false);
        } catch (Exception e) when (e is not FileError) {
            throw new FileError(Name, e);
        }
    }
    
    public void Dispose() {
        _doc.Dispose();
        GC.SuppressFinalize(this);
    }
    
    ~ExcelFile() {
        Dispose();
    }
    
}