using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using Core.Utils.Errors;
using UI.Services;
using UI.Utils;
using ApplicationException = System.ApplicationException;

namespace UI.ViewModels.Questions.Powerpoint;

public abstract class PowerpointQuestionViewModel(AbstractQuestion q, Evaluator evaluator, IErrorHandlerService errorHandler, IStorageService storageService) 
    : SingleQuestionViewModel(q, evaluator, errorHandler, storageService) {
    
    public override string Icon => "/Assets/pptx.svg";
    
    public override async Task AddFiles() {
        try {
            var pickerFiles =  await StorageService.GetFilesAsync(new FilePickerOpenOptions {
                AllowMultiple = true,
                FileTypeFilter = [FileTypesHelper.Powerpoint]
            });
            
            IFile[] files = await Task.WhenAll(pickerFiles
                .Select(async f => {
                    if ((await f.GetBasicPropertiesAsync()).Size > IFile.MaxBytesFileSize)
                        throw new FileTooLarge();
                    try {
                        await using var stream = await f.OpenReadAsync();
                        return new PowerpointFile(f.Name, stream);
                    } catch (Exception e) when (e is not ApplicationException) { throw new FileError(f.Name, e); }
                })
                .ToArray()
            );
            
            if (files.Length == 0)
                return;
            Evaluator.AddFiles(Question, files);
        } catch (FileError e) {
            UIException ex = e;
            ErrorHandler.ShowError(ex);
        }
    }
    
}