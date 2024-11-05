namespace Core.FileHandling; 

public interface IFile : IDisposable {
    protected string FileFormat { get; }
}