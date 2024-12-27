namespace Core.FileHandling;

public interface IFile : IDisposable {
    public const ulong MaxBytesFileSize = 200000000L; // 200 MB
    public string Name { get; }
}