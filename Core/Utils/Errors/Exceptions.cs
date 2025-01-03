namespace Core.Utils.Errors;

public class ApplicationException(string? msg = null, Exception? inner = null) : Exception(msg, inner);

public class FileError(string filename, Exception? inner) : ApplicationException(null, inner) {
    public string Filename => filename;
}

public class InvalidFileFormat(string filename) : FileError(filename, null);
public class FileAlreadyOpened(string path) : FileError(path, null);