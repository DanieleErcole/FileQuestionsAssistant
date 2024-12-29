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

    public bool IsCorrectFile { get; set; }
    public DocumentType DocumentTypeToSelect { get; set; } = DocumentType.Question;

    private string GetFilePath() => DocumentTypeToSelect switch {
        DocumentType.Word => IsCorrectFile ? "Document1.docx" : "InvalidFormat.docx",
        DocumentType.Excel => IsCorrectFile ? "Spreadsheet1.xlsx" : "InvalidFormat.xlsx",
        DocumentType.Powerpoint => IsCorrectFile ? "Presentation1.pptx" : "InvalidFormat.pptx",
        DocumentType.Question => IsCorrectFile ? "Domanda1.json" : "DomandaSbagliata.json"
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