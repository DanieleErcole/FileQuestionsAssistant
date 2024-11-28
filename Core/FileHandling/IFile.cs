namespace Core.FileHandling;

public interface IFile : IDisposable {
    public string Name { get; }
}