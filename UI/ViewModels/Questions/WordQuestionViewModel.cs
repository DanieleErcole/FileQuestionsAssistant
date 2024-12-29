using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using Core.Utils.Errors;
using Microsoft.Extensions.DependencyInjection;
using UI.Services;
using UI.Utils;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace UI.ViewModels.Questions;

public abstract class WordQuestionViewModel(AbstractQuestion q, Evaluator evaluator, IErrorHandlerService errorHandler, IStorageService storageService) 
    : SingleQuestionViewModel(q, evaluator, errorHandler, storageService) {
    
    public override string Icon => "/Assets/docx.svg";
    
    public override async Task AddFiles() {
        try {
            var pickerFiles =  await StorageService.GetFilesAsync(new FilePickerOpenOptions {
                AllowMultiple = true,
                FileTypeFilter = [FileTypesHelper.Word]
            });
            
            IFile[] files = await Task.WhenAll(pickerFiles
                .Select(async f => {
                    if ((await f.GetBasicPropertiesAsync()).Size > IFile.MaxBytesFileSize)
                        throw new FileTooLarge();
                    try {
                        await using var stream = await f.OpenReadAsync();
                        return new WordFile(f.Name, stream);
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