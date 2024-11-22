using System;
using Core.Utils.Errors;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace UI;

public class UIException(string msg) : Exception(msg) {
    public static implicit operator UIException(ApplicationException ex) {
        if (ex is FileError fileError) return new UIException(fileError.InnerException!.Message);
        return ex is InvalidFileFormat ? new UIException(Lang.Lang.InvalidFileFormatMsg) : new UIException(ex.Message);
    }

    public override string ToString() {
        return Message;
    }
}