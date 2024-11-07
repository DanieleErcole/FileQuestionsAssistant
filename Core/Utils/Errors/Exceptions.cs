namespace Core.Utils.Errors;

public class ApplicationException(string? msg = null, Exception? inner = null) : Exception(msg, inner);

public class InvalidFileFormat : ApplicationException;
public class FileError(Exception inner) : ApplicationException(null, inner);