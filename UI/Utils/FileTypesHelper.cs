using Avalonia.Platform.Storage;

namespace UI.Utils;

public static class FileTypesHelper {
    public static readonly FilePickerFileType Word = new("Word OpenXML files") {
        Patterns = ["*.docx"],
        AppleUniformTypeIdentifiers = ["org.openxmlformats.wordprocessingml.document"],
        MimeTypes = ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"]
    };
    
    public static readonly FilePickerFileType Excel = new("Excel OpenXML files") {
        Patterns = ["*.xlsx"],
        AppleUniformTypeIdentifiers = ["org.openxmlformats.spreadsheetml.sheet"],
        MimeTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]
    };

    public static readonly FilePickerFileType Powerpoint = new("Powerpoint OpenXML files") {
        Patterns = ["*.pptx"],
        AppleUniformTypeIdentifiers = ["org.openxmlformats.presentationml.presentation"],
        MimeTypes = ["application/vnd.openxmlformats-officedocument.presentationml.presentation"]
    };
}