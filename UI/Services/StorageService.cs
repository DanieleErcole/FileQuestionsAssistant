using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.FileHandling;
using Core.Utils.Errors;
using UI.Utils;
using ApplicationException = System.ApplicationException;

namespace UI.Services;

public class StorageService(IStorageProvider storageProvider) : IStorageService {
    
    public Task<IReadOnlyList<IStorageFile>> GetFilesAsync(FilePickerOpenOptions? options = null) 
        => storageProvider.OpenFilePickerAsync(options ?? new FilePickerOpenOptions());

    public Task<IStorageFile?> SaveFileAsync(FilePickerSaveOptions? options = null) 
        => storageProvider.SaveFilePickerAsync(options ?? new FilePickerSaveOptions());

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
                }catch (Exception e) when (e is not FileError) {
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