using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.FileHandling;

namespace UI.Services;

public interface IStorageService {
    Task<IReadOnlyList<IStorageFile>> GetFilesAsync(FilePickerOpenOptions? options = null);
    Task<IStorageFile?> SaveFileAsync(FilePickerSaveOptions? options = null);
    Task<IFile[]> GetFilesOfTypeAsync(FilePickerFileType type, bool allowMultiple = false);
}