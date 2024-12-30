using Avalonia.Platform.Storage;
using Tests.Utils;
using UI.Services;

namespace Tests.TestApp.Services;

public class TestStorageService(IStorageProvider storageProvider) : IStorageService {

    public enum DocumentType {
        Word,
        Excel,
        Powerpoint,
        Question
    }

    public bool? IsCorrect { get; set; } = false;
    public DocumentType DocumentTypeToSelect { get; set; } = DocumentType.Question;

    private string GetFilePath() => DocumentTypeToSelect switch {
        DocumentType.Word => IsCorrect switch {
            true => "Document1.docx",
            false => "InvalidFormat.docx",
            _ => "OgFile.docx"
        },
        DocumentType.Excel => IsCorrect switch {
            true => "Spreadsheet1.xlsx",
            false => "InvalidFormat.xlsx",
            _ => "OgSpreadsheet.xlsx"
        },
        DocumentType.Powerpoint => IsCorrect switch {
            true => "Presentation1.pptx",
            false => "InvalidFormat.pptx",
            _ => "OgPresentation.pptx"
        },
        DocumentType.Question => (bool) IsCorrect! ? "Domanda1.json" : "DomandaSbagliata.json"
    };

    public async Task<IReadOnlyList<IStorageFile>> GetFilesAsync(FilePickerOpenOptions? options = null) {
        var res = await storageProvider.TryGetFileFromPathAsync($"{TestConstants.TestFilesDirectory}{GetFilePath()}");
        return res is null ? Array.Empty<IStorageFile>() : new List<IStorageFile> { res };
    }

    public async Task<IStorageFile?> SaveFileAsync(FilePickerSaveOptions? options = null) {
        await using var _ = File.Open($"{TestConstants.TestFilesDirectory}FileToSave.json", FileMode.Create);
        return await storageProvider.TryGetFileFromPathAsync($"{TestConstants.TestFilesDirectory}FileToSave.json");
    }
    
}