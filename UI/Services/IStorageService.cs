using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace UI.Services;

public interface IStorageService {
    Task<IReadOnlyList<IStorageFile>> GetFilesAsync(FilePickerOpenOptions? options = null);
    Task<IStorageFile?> SaveFileAsync(FilePickerSaveOptions? options = null);
}