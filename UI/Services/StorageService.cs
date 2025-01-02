using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace UI.Services;

public class StorageService(IStorageProvider storageProvider) : IStorageService {
    
    public Task<IReadOnlyList<IStorageFile>> GetFilesAsync(FilePickerOpenOptions? options = null) 
        => storageProvider.OpenFilePickerAsync(options ?? new FilePickerOpenOptions());

    public Task<IStorageFile?> SaveFileAsync(FilePickerSaveOptions? options = null) 
        => storageProvider.SaveFilePickerAsync(options ?? new FilePickerSaveOptions());
    
}