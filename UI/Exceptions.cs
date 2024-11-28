using System;
using Core.Utils.Errors;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace UI;

public class UIException(string title, string desc, Exception? inner = null) : Exception(null, inner) {

    public string Title { get; } = title;
    public string Desc { get; } = desc;

    public static UIException FromException(Exception e) {
        return new UIException("Unknown error", e.InnerException?.Message ?? "");
    }

    public static implicit operator UIException(ApplicationException ex) {
        return ex switch {
            InvalidFileFormat => new UIException("Error opening file", Lang.Lang.InvalidFileFormatMsg),
            FileError other => other.InnerException switch {
                UnauthorizedAccessException => new UIException("Unable to access the file", "You do not have permission to access this file"),
                _ => new UIException($"Error opening file: {other.Filename}", other.InnerException?.Message ?? "")
            },
            _ => new UIException("Unknown error", ex.InnerException?.Message ?? "")
        };
    }

    public override string ToString() {
        return InnerException is null ? Message : $"{Message}: {InnerException}";
    }
    
}

public class UnableToOpenQuestion() : UIException("Unable to open question", "Question already tracked");