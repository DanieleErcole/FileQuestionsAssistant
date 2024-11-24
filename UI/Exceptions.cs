using System;
using Core.Utils.Errors;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace UI;

public class UIException(string msg, Exception? inner = null) : Exception(msg, inner) {
    
    public static implicit operator UIException(ApplicationException ex) {
        return ex switch {
            InvalidFileFormat => new UIException(Lang.Lang.InvalidFileFormatMsg),
            FileError other => new UIException("Unknown file error", other.InnerException),
            _ => new UIException("Unknown error", ex.InnerException)
        };
    }
    
    public static implicit operator UIException(UnauthorizedAccessException _) {
        return new UIException("You do not have permission to access this file");
    }

    public override string ToString() {
        return InnerException is null ? Message : $"{Message}: {InnerException}";
    }
}