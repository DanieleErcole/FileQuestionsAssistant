using Avalonia.Platform.Storage;
using Core.FileHandling;
using Core.Utils.Errors;
using Tests.Utils;
using UI.Services;
using UI.Utils;
using ApplicationException = System.ApplicationException;

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
    
    public async Task<IFile[]> GetFilesOfTypeAsync(FilePickerFileType type, bool allowMultiple = false) {
        var pickerFiles = await GetFilesAsync(new FilePickerOpenOptions {
            AllowMultiple = allowMultiple,
            FileTypeFilter = [type]
        });
            
        return await Task.WhenAll(
            pickerFiles.Select(async f => { 
                if ((await f.GetBasicPropertiesAsync()).Size > IFile.MaxBytesFileSize)
                    throw new FileTooLarge();
                try {
                    await using var stream = await f.OpenReadAsync();
                    return FileFromType(type, f.Name, stream);
                }catch (Exception e) when (e is not ApplicationException) {
                    throw new FileError(f.Name, e);
                }
            }).ToArray()
        );
    }

    private IFile FileFromType(FilePickerFileType type, string name, Stream stream) {
        if (type == FileTypesHelper.Word)
            return new WordFile(name, stream);
        if (type == FileTypesHelper.Powerpoint)
            return new PowerpointFile(name, stream);
        if (type == FileTypesHelper.Excel)
            return new ExcelFile(name, stream);
        throw new ArgumentException("Invalid file type");
    }
    
}