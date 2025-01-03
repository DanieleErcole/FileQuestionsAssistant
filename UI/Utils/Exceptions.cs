using System;
using Core.FileHandling;
using Core.Utils.Errors;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace UI.Utils;

public class UIException(string title, string desc, Exception? inner = null) : Exception(null, inner) {

    public string Title { get; } = title;
    public string Desc { get; } = desc;

    public static UIException FromException(Exception e) {
        return e switch {
            ApplicationException appEx => appEx,
            _ => new UIException(Lang.Lang.UnknownError, e.InnerException?.Message ?? "")
        };
    }

    public static implicit operator UIException(ApplicationException ex) {
        return ex switch {
            FileAlreadyOpened e => new UIException(Lang.Lang.ErrorOpeningFilename, string.Format(Lang.Lang.FileAlreadyOpenedMsg, e.Filename)),
            InvalidFileFormat => new UIException(Lang.Lang.ErrorOpeningFilename, Lang.Lang.InvalidFileFormatMsg),
            FileError other => other.InnerException switch {
                UnauthorizedAccessException => new UIException(Lang.Lang.UnableToAccessFile, Lang.Lang.NoPermissionDesc),
                _ => new UIException(Lang.Lang.ErrorOpeningFilename + $": {other.Filename}", other.InnerException?.Message ?? "")
            },
            _ => new UIException(Lang.Lang.UnknownError, ex.InnerException?.Message ?? "")
        };
    }

    public override string ToString() {
        return InnerException is null ? Message : $"{Message}: {InnerException}";
    }
    
}

public class UnableToOpenQuestion() : UIException(Lang.Lang.UnableToOpenQuestion, Lang.Lang.UnableToOpenQuestionDesc);
public class FileTooLarge() : UIException(Lang.Lang.FileTooLargeTitle, $"{Lang.Lang.FileTooLargeDesc} {IFile.MaxBytesFileSize}");