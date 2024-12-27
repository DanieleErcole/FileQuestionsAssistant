using Avalonia.Platform.Storage;
using Tests.Utils;
using UI.Services;

namespace Tests.TestApp.Services;

public class TestStorageService(IStorageProvider storageProvider) : IStorageService {

    public bool IsCorrectFile { get; set; }

    public async Task<IReadOnlyList<IStorageFile>> GetFilesAsync(FilePickerOpenOptions? options = null) {
        var res = await storageProvider.TryGetFileFromPathAsync($"{TestConstants.TestFilesDirectory}{(IsCorrectFile 
                ? "Domanda1.json" 
                : "DomandaSbagliata.json"
            )}");
        return res is null ? Array.Empty<IStorageFile>() : new List<IStorageFile> { res };
    }

    public async Task<IStorageFile?> SaveFileAsync(FilePickerSaveOptions? options = null) {
        await using var _ = File.Open($"{TestConstants.TestFilesDirectory}FileToSave.json", FileMode.Create);
        return await storageProvider.TryGetFileFromPathAsync($"{TestConstants.TestFilesDirectory}FileToSave.json");
    }
    
}